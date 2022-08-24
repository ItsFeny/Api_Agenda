using Api_Agenda.Models;
using Api_Agenda.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Api_Agenda.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService; 
       
       
        public AuthController(IConfiguration configuration, IAuthService authService)
        {
            _configuration = configuration;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> register([FromBody] UserRegister request)
        {
            CreatePasswordHash(request.password, out byte[] passwordHash, out byte[] passwordSalt);

            user.Name = request.Name;
            user.Email = request.Email;
            user.password = request.password;
            user.passwordHash = passwordHash;
            user.passwordSalt = passwordSalt;

            await _authService.SignUp(user);
            return Ok(user);
        }


        [HttpPost("login")]
        public ActionResult login([FromBody] UserLogin request)
        {
            var userSV = _authService.GetUserForSignIn(request.Name);

            if (userSV is null)  return BadRequest("Usuerio No Encontrado");
           

            if(!verifyPasswordHash(request.Password, userSV.passwordHash, userSV.passwordSalt))
            {
                return BadRequest("Contrasena Incorrecta!");
            }

            string token = CreateToken(userSV);
            LoginResponse loginResponse = new();
            loginResponse.Token = token;

            return Ok(loginResponse);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name)
              
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

      

        private bool verifyPasswordHash(string password,  byte[] passwordHash,  byte[] passwordSalt)
        {
           
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }

        }
    }

    public partial class LoginResponse
    {
        public string Token { get; set; }
    }
}
