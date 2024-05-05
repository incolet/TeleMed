using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeleMed.DTOs.Auth;
using TeleMed.Repos.Abstracts;
using TeleMed.States;

namespace TeleMed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController(IEnumerable<IAccount> accountRepos) : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto model)
        {
            
            var accountRepo = accountRepos.FirstOrDefault(x => x.GetType().Name == Enum.GetName(typeof(UserRoles),model.Role) + "Account");
            var user = accountRepo!.LoginAsync(model);
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
            var user = accountRepos.FirstOrDefault()!.RefreshToken(userSession);
            if (!user.Flag)
            {
                return BadRequest(new { message = "Invalid token" });
            }
            return Ok(user);
        }
        
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto model)
        {
            var user = accountRepos.FirstOrDefault()!.RegisterAsync(model);
            if (!user.Item1.Flag)
            {
                return BadRequest(new { message = "Username or email is already taken" });
            }
            return Ok(user);
        }

    }
}
