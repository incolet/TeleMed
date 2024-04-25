using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeleMed.DTOs.Auth;
using TeleMed.Repos.Abstracts;

namespace TeleMed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController(IAccount accountRepo) : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto model)
        {
            var user = accountRepo.LoginAsync(model);
            if (!user.Flag)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            return Ok(user);
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public IActionResult RefreshToken([FromBody] UserSession userSession)
        {
            var user = accountRepo.RefreshToken(userSession);
            if (!user.Flag)
            {
                return BadRequest(new { message = "Invalid token" });
            }
            return Ok(user);
        }
        
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto model)
        {
            var user = accountRepo.RegisterAsync(model);
            if (!user.Item1.Flag)
            {
                return BadRequest(new { message = "Username or email is already taken" });
            }
            return Ok(user);
        }

    }
}
