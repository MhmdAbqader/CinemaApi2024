using DevCreedApi_Net6Movies.Models;
using DevCreedApi_Net6Movies.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevCreedApi_Net6Movies.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationSevice _authSevice;

        public AccountController(IAuthenticationSevice authSevice) 
        {
            this._authSevice = authSevice;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody]RegisterModel model) {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);
            

            var result= await _authSevice.Register(model);
            if(!result.IsAuthenticated)
                return BadRequest(result.Msg);
            //i can choose the values needed only if i use anonymous object
         return Ok(new { isauth=result.IsAuthenticated,Token=result.Token, expire=result.ExpireOn});
       // return Ok(result);

        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginModel model) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           

            var result = await _authSevice.GetLogin(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Msg);

            if (result.RefreshToken != null)
                SetRefreshTokenInCookies(result.RefreshToken , result.RefreshTokenExpirationOn);

            // return Ok(new { isauth = result.IsAuthenticated, Token = result.Token, expire = result.ExpireOn });
            return Ok(result);
        }

        [HttpGet("RefreshTokenAsync")]
        public async Task<IActionResult> RefreshTokenAsync() 
        {
            var tokenSavedInCookies = HttpContext.Request.Cookies["RerToken"];
            var result = await _authSevice.RefreshTokenAsync(tokenSavedInCookies);
            if(result.IsAuthenticated == false)
                return BadRequest(result.Msg);

            SetRefreshTokenInCookies(result.RefreshToken, result.RefreshTokenExpirationOn);
            return Ok(result);
        }

        [HttpPost("RevokeTokenAsync")]
        public async Task<IActionResult> RevokeTokenAsync([FromBody] RevokeTokenModel revokeTokenModel)
        {
            var tokenRequiredForRevoking = revokeTokenModel.Token ==null ? HttpContext.Request.Cookies["RerToken"]: revokeTokenModel.Token;

            if (string.IsNullOrEmpty(tokenRequiredForRevoking)) 
            {
                return BadRequest("Token is required");
            }
            var result = await _authSevice.RevokeTokenAsync(tokenRequiredForRevoking);
            if (result == false)
                return BadRequest("InValid Token!!!");
 
            return Ok(result);
        }
        private void SetRefreshTokenInCookies(string refreshToken, DateTime refreshTokenExpirationOn)
        {
            CookieOptions cookieOptions = new()
            {
                HttpOnly = true,
                Expires = refreshTokenExpirationOn
            };
            HttpContext.Response.Cookies.Append("RerToken", refreshToken, cookieOptions);
        }

        [HttpPost("SetRole")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> SetRoleToUser(SetRoleModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var result = await _authSevice.SetRoleToUser(model);
            if (result.Contains("Error"))
                return BadRequest(result);

            // return Ok(new { isauth = result.IsAuthenticated, Token = result.Token, expire = result.ExpireOn });
            return Ok(result);
        }

    }
}
