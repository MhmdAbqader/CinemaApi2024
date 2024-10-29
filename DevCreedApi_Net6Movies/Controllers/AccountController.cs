using DevCreedApi_Net6Movies.Models;
using DevCreedApi_Net6Movies.Services;
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
        public async Task<IActionResult> tokenLogin(LoginModel model) {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var result = await _authSevice.GetLogin(model);
            if (!result.IsAuthenticated)
                return BadRequest(result.Msg);

            // return Ok(new { isauth = result.IsAuthenticated, Token = result.Token, expire = result.ExpireOn });
            return Ok(result);
        }
    }
}
