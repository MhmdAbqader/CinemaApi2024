using DevCreedApi_Net6Movies.Helper;
using DevCreedApi_Net6Movies.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace DevCreedApi_Net6Movies.Services
{
    public class AuthenticationSevice : IAuthenticationSevice
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _access;
        private readonly JWT _jwt;

        public AuthenticationSevice(UserManager<ApplicationUser> userManager,RoleManager<IdentityRole> roleManager,
            IOptions<JWT> jwt, IHttpContextAccessor access)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _roleManager = roleManager;
            _access = access;
        }


        public async Task<AuthenticationModel> Register(RegisterModel registerModel)
        {
            var ExistEmail = await _userManager.FindByEmailAsync(registerModel.Email);
            var ExistUsername = await _userManager.FindByNameAsync(registerModel.Username);

            if (ExistEmail != null)
                return new AuthenticationModel { Msg = "Email Is Already Exists!" };

            if (ExistUsername != null)
                return new AuthenticationModel {Msg= "UserName Is Already Exists!" };

            var user = new ApplicationUser
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Email = registerModel.Email,
                UserName = registerModel.Username
            };

            var result = await _userManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                string err = string.Empty;
                foreach (var iterationError in result.Errors) {
                    err += iterationError.Description + "-";
                }
                return new AuthenticationModel{ Msg = err };
            }

            await _userManager.AddToRoleAsync(user,"Admin");
            var jwtSecurityToken= await CreateJWTToken(user);

            return new AuthenticationModel {
                Email = user.Email,
                ExpireOn = jwtSecurityToken.ValidTo,
                Username=user.UserName,
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Roles=new List<string> { "User"}
            };   
        }

        public async Task<JwtSecurityToken> CreateJWTToken(ApplicationUser user) {

            var userclaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleclaims= new List<Claim>();
            foreach (var role in roles) {
                //roleclaims.Add(new Claim("roles",role));
                 roleclaims.Add(new Claim(ClaimTypes.Role,role));
            }

            var cliams = new[] {
                 new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                 new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                 new Claim(JwtRegisteredClaimNames.Email,user.Email),
                 new Claim(ClaimTypes.NameIdentifier,user.Id)
                 //new Claim("uid",user.Id)
            }.Union(userclaims).Union(roleclaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.key));
            var signingcredentials = new SigningCredentials(symmetricSecurityKey,SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.issuer,
                audience:_jwt.audience,
                claims: cliams,
                expires:DateTime.Now.AddMinutes(_jwt.expireIn),
                signingCredentials: signingcredentials
                ) ;
         
            return jwtSecurityToken;
        }

        public async Task<AuthenticationModel> GetLogin(LoginModel model)
        {
            var authModel= new AuthenticationModel();
            var user = await _userManager.FindByEmailAsync(model.Email);
            bool checkPassword = await _userManager.CheckPasswordAsync(user, model.Password);
            if (user == null || !checkPassword)
            {
                // authModel.Msg = "Email or Password IS Invalid !";
                //return authModel;
                return new AuthenticationModel { Msg= "Email or Password IS Invalid !" };
            }

            var jwtSecurityToken = await CreateJWTToken(user);
            var roleList = await _userManager.GetRolesAsync(user);
             

            authModel.IsAuthenticated = true;
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpireOn = jwtSecurityToken.ValidTo;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Roles = roleList.ToList();

            if (user.RefreshTokens.Any(a=>a.IsActive)) 
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(a=>a.IsActive);
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpirationOn = activeRefreshToken.ExpireOn; 
            }
            else
            {
                // call function that make refreshtoken then save in Db  
                var newRefreshToken = GenerateRefreshToken();
                authModel.RefreshToken = newRefreshToken.Token;
                authModel.RefreshTokenExpirationOn = newRefreshToken.ExpireOn;
                user.RefreshTokens.Add(newRefreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authModel;

        }

        public async Task<AuthenticationModel> RefreshTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(a => a.RefreshTokens.Any(t => t.Token == token));
            var authModel = new AuthenticationModel();

            if (user == null)
            {
                authModel.Msg = "Invalid Token!";
                authModel.IsAuthenticated = false;
                return authModel;
            }
            var refershToken = user.RefreshTokens.Single(t => t.Token == token);
            if (refershToken.IsActive == false) 
            {
                authModel.Msg = "InActive Token!";
                authModel.IsAuthenticated = false;
                return authModel;
            }

            refershToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJWTToken(user);

            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpirationOn = newRefreshToken.ExpireOn;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken); 
            authModel.IsAuthenticated = true;
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpireOn = newRefreshToken.ExpireOn ;
            var roles =await _userManager.GetRolesAsync(user);
            authModel.Roles = roles.ToList();


            return authModel;
            
        }

        public  async Task<bool> RevokeTokenAsync(string token) 
        {
           
            var user = await _userManager.Users.SingleOrDefaultAsync(a => a.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return false;
            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            // in case actiVe==false =>>>> revoke is not null or already expired 
            // if expired i can't revoke it because actually expired
            if (refreshToken.IsActive == false)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return true;
        }

        private RefreshToken GenerateRefreshToken ()
        {
            var randomNo = new byte[32];

            using var generator = new System.Security.Cryptography.RNGCryptoServiceProvider(); 
            generator.GetBytes(randomNo);
            RefreshToken refToken = new RefreshToken()
            {
                Token = Convert.ToBase64String(randomNo),
                CreatedOn = DateTime.UtcNow,
                ExpireOn = DateTime.UtcNow.AddMinutes(5),
            };

            return refToken;
        }


        public async Task<string> SetRoleToUser(SetRoleModel setRole)
        {

            string whoISUser = GetCurrentUser();

            if (whoISUser.Contains("MhmdAbqader@yahoo"))
            {

                var isValidUser = await _userManager.FindByIdAsync(setRole.UserId);
                var isValidRole = await _roleManager.FindByNameAsync(setRole.RoleName);

                if (isValidUser == null)
                    return "Invalid User Id";

                if (isValidRole == null)
                    return "Invalid Role Name";
                if (await _userManager.IsInRoleAsync(isValidUser, setRole.RoleName))
                    return "user already in Role Name";

                var result = await _userManager.AddToRoleAsync(isValidUser, "User");
                if (result.Succeeded)
                    return "user added to Role";

                return "Error Occured";
            }
            else
                return "Sorry, Not Allowed u Aren't the Owner";
        }

        private string GetCurrentUser() 
        {
            // Retrieve the current user's claims
            var claims = _access.HttpContext.User.Claims;

            // Example: Get the user's email from the claims
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            return email;

        }

    
    }
}
