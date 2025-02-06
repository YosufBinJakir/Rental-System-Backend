using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using RentalSystem.Models;
using RentalSystem.ViewModels;
using System.Text;

namespace RentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles ="Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RentsDbContext _context;

        public AdminController(UserManager<AppUser> userManager,
                              SignInManager<AppUser> signInManager,
                              IConfiguration configuration,
                              RoleManager<IdentityRole> roleManager,
                              RentsDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _context = context;
        }


        [HttpPost("post-category")]/// for admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostCategory([FromBody] CategoryVM categoryInput, [FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(token) || !token.StartsWith("Bearer "))
                {
                    return BadRequest("Invalid or missing token.");
                }

               
                var jwtToken = token.Substring("Bearer ".Length).Trim();

               
                var claims = GetUserIdFromToken(jwtToken);
                if (string.IsNullOrEmpty(claims.Id))
                {
                    return Unauthorized("User ID not found in token.");
                }


                var category = new Category
                {
                    CategoryName = categoryInput.CategoryName,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    LastModifiedByUserId = claims.Id,
                    LastModifiedAt = DateTime.UtcNow,
                    CreatedByUserId = claims.Id,
                    AppUserId = claims.Id,
                    AppUserName =claims.UserName
                };


                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error in PostCategory: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("category-list")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetCategoryList()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories);
        }

        [HttpGet("applications")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetApplies()
        {
            var applies = await _context.Applications.ToListAsync();
            return Ok(applies);
        }


        //[HttpPost]
        //public async Task<int> PostApply(int postId)
        //{

        //}



        public static class JwtHelper
        {
            public static string Base64UrlDecode(string input)
            {
                input = input.Replace('-', '+').Replace('_', '/');
                switch (input.Length % 4)
                {
                    case 2: input += "=="; break;
                    case 3: input += "="; break;
                }
                return Encoding.UTF8.GetString(Convert.FromBase64String(input));
            }
        }

        public JwtClaims GetUserIdFromToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    throw new ArgumentException("Token is null or empty.");
                }

                var tokenParts = token.Split('.');
                if (tokenParts.Length != 3)
                {
                    throw new FormatException("Invalid token structure.");
                }

                var payload = tokenParts[1];
                var jsonPayload = JwtHelper.Base64UrlDecode(payload);

                var jwtPayload = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

                return new JwtClaims
                {
                    Id = jwtPayload.ContainsKey("sub") ? jwtPayload["sub"] : null,
                    UserName = jwtPayload.ContainsKey("name") ? jwtPayload["name"] : null,
                    Email = jwtPayload.ContainsKey("email") ? jwtPayload["email"] : null,
                    PhoneNumber= jwtPayload.ContainsKey("phone_number") ? jwtPayload["phone_number"] :null,
                    Role = jwtPayload.ContainsKey("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
                        ? jwtPayload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
                        : null
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding token: {ex.Message}");
                throw; 
            }
        }

    }
}
