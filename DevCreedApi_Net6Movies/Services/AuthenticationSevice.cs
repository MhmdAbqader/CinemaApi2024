using DevCreedApi_Net6Movies.Helper;
using DevCreedApi_Net6Movies.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DevCreedApi_Net6Movies.Services
{
    public class AuthenticationSevice : IAuthenticationSevice
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly JWT _jwt;

        public AuthenticationSevice(UserManager<ApplicationUser> userManager , Microsoft.Extensions.Options.IOptions<JWT> jwt)
        {
            this._UserManager = userManager;
            this._jwt = jwt.Value;
        }


        public async Task<AuthenticationModel> Register(RegisterModel registerModel)
        {
            var ExistEmail = await _UserManager.FindByEmailAsync(registerModel.Email);
            var ExistUsername = await _UserManager.FindByNameAsync(registerModel.Username);

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

            var result = await _UserManager.CreateAsync(user, registerModel.Password);

            if (!result.Succeeded)
            {
                string err = string.Empty;
                foreach (var iterationError in result.Errors) {
                    err += iterationError.Description + "-";
                }
                return new AuthenticationModel{ Msg = err };
            }

            await _UserManager.AddToRoleAsync(user,"Admin");
            var jwtSecurityToken= await CreateToken(user);

            return new AuthenticationModel {
                Email = user.Email,
                ExpireOn = jwtSecurityToken.ValidTo,
                Username=user.UserName,
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Roles=new List<string> { "User"}
            };   
        }

        public async Task<JwtSecurityToken> CreateToken(ApplicationUser user) {

            var userclaims = await _UserManager.GetClaimsAsync(user);
            var roles = await _UserManager.GetRolesAsync(user);
            var roleclaims= new List<Claim>();
            foreach (var role in roles) {
                roleclaims.Add(new Claim("roles",role));
            }

            var cliams = new[] {
                 new Claim(JwtRegisteredClaimNames.Sub,user.UserName),
                 new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                 new Claim(JwtRegisteredClaimNames.Email,user.Email),
                 new Claim("uid",user.Id)
            }.Union(userclaims).Union(roleclaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.key));
            var signingcredentials = new SigningCredentials(symmetricSecurityKey,SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.issuer,
                audience:_jwt.audience,
                claims: cliams,
                expires:DateTime.Now.AddDays(_jwt.ExpireInDays),
                signingCredentials: signingcredentials
                ) ;
         
            return jwtSecurityToken;
        }

        public async Task<AuthenticationModel> GetLogin(LoginModel model)
        {
            var authModel= new AuthenticationModel();
            var user = await _UserManager.FindByEmailAsync(model.Email);
            bool checkPassword = await _UserManager.CheckPasswordAsync(user, model.Password);
            if (user == null || !checkPassword)
            {
                // authModel.Msg = "Email or Password IS Invalid !";
                //return authModel;
                return new AuthenticationModel { Msg= "Email or Password IS Invalid !" };
            }
            var jwtSecurityToken = await CreateToken(user);
            var roleList = await _UserManager.GetRolesAsync(user);
             

            authModel.IsAuthenticated = true;
            authModel.Email = user.Email;
            authModel.Username = user.UserName;
            authModel.ExpireOn = jwtSecurityToken.ValidTo;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            authModel.Roles = roleList.ToList();

            return authModel;

        }
    }
}
