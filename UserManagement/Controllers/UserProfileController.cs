using Microsoft.AspNetCore.Mvc;
using UserManagement.DTO;
using UserManagement.Repositories;

namespace UserManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : Controller
    {
        private readonly UserManagementDbContext _context;

        public UserProfileController(UserManagementDbContext context)
        {
            _context = context;
        }

        private UserProfile GetUserProfile(int id) => _context.UserProfiles.SingleOrDefault(x => x.Id == id)
                                                             ?? throw new ArgumentNullException(nameof(id));

        [HttpGet("getAll")]
        public IActionResult Get()
        {
            try
            {
                return Ok(_context.UserProfiles.ToArray());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("getById/{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                return Ok(GetUserProfile(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("insert")]
        public IActionResult Insert([FromBody] UserProfile value)
        {
            try
            {
                _context.UserProfiles.Add(value);
                _context.SaveChanges();
                return Ok("YAY! user profile inserted!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update/{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] UserProfile userProfile)
        {
            if (userProfile == null) throw new ArgumentNullException(nameof(userProfile));

            try
            {
                var existingProfile = GetUserProfile(id);

                if (existingProfile == null) return NotFound($"UserProfile with ID {id} not found");

                existingProfile.Firstname = userProfile.Firstname;
                existingProfile.LastName = userProfile.LastName;
                existingProfile.PersonalNumber = userProfile.PersonalNumber;

                _context.UserProfiles.Update(existingProfile);
                _context.SaveChanges();

                return Ok("YAY! user profile updated!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var profile = GetUserProfile(id);

            try
            {
                User? user = _context.Users.SingleOrDefault(x => x.Id == id);

                if (user != null)
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                }

                _context.UserProfiles.Remove(profile);
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
