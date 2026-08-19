using Microsoft.AspNetCore.Mvc;
using ToDoList.Repositories;
using ToDoList.Security;

namespace ToDoList.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository userRepository;
        private readonly JwtTokenGenerator tokenGenerator;

        public AuthController(UserRepository userRepository, JwtTokenGenerator tokenGenerator)
        {
            this.userRepository = userRepository;
            this.tokenGenerator = tokenGenerator;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var user = userRepository.Register(request.Username, request.Password);

            if (user == null)
                return BadRequest("Username already taken");

            return Ok(new { user.Id, user.Username });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = userRepository.Login(request.Username, request.Password);

            if (user == null)
                return Unauthorized("Invalid username or password");

            var token = tokenGenerator.GenerateToken(user);

            return Ok(new { token });
        }
    }

    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}