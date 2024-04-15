using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TeleMed.DTOs;
using TeleMed.Repos;
using TeleMed.Repos.Abstracts;

namespace TeleMed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccount accountRepo;
        public AccountsController(IAccount accountRepo)
        {
            this.accountRepo = accountRepo;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO model)
        {
            var user = accountRepo.LoginAsync(model);
            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }
            return Ok(user);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDTO model)
        {
            var user = accountRepo.RegisterAsync(model);
            if (user.Item1 == null)
            {
                return BadRequest(new { message = "Username or email is already taken" });
            }
            return Ok(user);
        }

    }
}
