using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagement.DTO;
using UserManagement.Facade;
using UserManagement.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserManagementDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(UserManagementDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private User GetUser(int id) => _context.Users.SingleOrDefault(x => x.Id == id)
                                         ?? throw new ArgumentNullException(nameof(id));

        [HttpPost("logIn")]
        public IActionResult LogIn(string username, string password)
        {

            var user = _context.Users.Where(a => a.Username == username).SingleOrDefault();
            var hashed = password.HashData();

            if (user == null || hashed != user.Password)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(ClaimTypes.Role, "UserProfile")
            };

            var token = new JwtSecurityToken
                (
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddDays(1),
                    notBefore: DateTime.UtcNow,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
                    SecurityAlgorithms.HmacSha256)
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(tokenString);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
        [HttpGet("getAll")]
        public IActionResult Get()
        {
            try
            {
                return Ok(_context.Users.ToArray());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpGet("getById/{id}")]
        public IActionResult Get([FromRoute] int id)
        {
            try
            {
                return Ok(GetUser(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("insert")]
        public IActionResult Insert([FromBody] User user)
        {
            try
            {
                _context.Users.Add(user);
                _context.SaveChanges();
                return Ok("YAY! user inserted!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            try
            {
                var existingUser = GetUser(id);

                if (existingUser == null) return NotFound($"UserProfile with ID {id} not found");

                existingUser.Username = user.Username;

                _context.Users.Update(existingUser);
                _context.SaveChanges();

                return Ok("YAY! user updated!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPut("delete/{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var profile = GetUser(id);

            try
            {
                profile.isActive = false;

                _context.Users.Update(profile);
                _context.SaveChanges();

                return Ok("user profile removed successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
