using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RentalSystem.Models;
using RentalSystem.ViewModels;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly RentsDbContext _context;

        public UserController(UserManager<AppUser> userManager,
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

        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] Register model)
        //{
        //    if (await _userManager.FindByEmailAsync(model.Email) != null)
        //        return BadRequest(new { message = "Email is already in use." });

        //    var user = new IdentityUser { UserName = model.UserName, Email = model.Email, PhoneNumber=model.PhoneNumber };
        //    var result = await _userManager.CreateAsync(user, model.Password);

        //    if (!result.Succeeded)
        //        return BadRequest(result.Errors);

        //    // Assign the "Member" role by default after registration
        //    await _userManager.AddToRoleAsync(user, "Member");

        //    return Ok(new { message = "Registration successful" });
        //}

    
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
           


            if (user == null) return Unauthorized(new { message = "Invalid email or password" });

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid email or password" });

            var token = GenerateJwtToken(user);
            return Ok(new { token });
        }


        [HttpGet("profile")]
        public async Task<IActionResult> AllUserProfile()
        {
            var profiles = await _context.UserProfiles.ToListAsync();
            if (profiles == null) return NotFound();
            return Ok(profiles);
        }


        [HttpGet("profile/{id}")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> GetUserProfile(string id)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(x => x.AppUserId == id);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

        //public string GetUserIdFromToken(string token)
        //{
        //    string userId;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(token))
        //        {
        //            var tokenParts = token.Split('.');
        //            if (tokenParts.Length > 1)
        //            {

        //                var payload = tokenParts[1];
        //                var base64Payload = payload.Replace('-', '+').Replace('_', '/');
        //                var jsonPayload = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(base64Payload));


        //                var jwtPayload = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

        //                if (jwtPayload.ContainsKey("sub"))
        //                {
        //                    userId = jwtPayload["sub"];
        //                }
        //                else
        //                {
        //                    throw new Exception("User ID not found in payload.");
        //                }
        //            }
        //            else
        //            {
        //                throw new Exception("Invalid token structure.");
        //            }
        //        }
        //        else
        //        {
        //            return ("Token is null or empty.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log exception
        //        Console.WriteLine(ex.Message);
        //        return ("Invalid token format or missing ID.");
        //    }

        //    return userId;
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

        public string GetUserIdFromToken(string token)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    var tokenParts = token.Split('.');
                    if (tokenParts.Length == 3)
                    {
                        var payload = tokenParts[1];
                        var jsonPayload = JwtHelper.Base64UrlDecode(payload);
                        var jwtPayload = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

                        if (jwtPayload.ContainsKey("sub"))
                        {
                            return jwtPayload["sub"];
                        }
                        else
                        {
                            throw new Exception("User ID not found in payload.");
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid token structure.");
                    }
                }
                else
                {
                    return "Token is null or empty.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "Invalid token format or missing ID.";
            }
        }










        [HttpPut("update-profile")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> PutProfile(UserProfileVM model, [FromHeader(Name = "Authorization")] string token)
        {
            var userId = GetUserIdFromToken(token);

            var existingProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.AppUserId == userId);

            if (existingProfile == null) return NotFound();

            existingProfile.ProfilePictureUrl = model.ProfilePictureUrl;
            existingProfile.City = model.City;
            existingProfile.Address = model.Address;
            existingProfile.Gender = model.Gender;
            existingProfile.DateOfBirth = model.DateOfBirth;
            existingProfile.FirstName = model.FirstName;
            existingProfile.LastName = model.LastName;
            existingProfile.Country = model.Country;
            existingProfile.CreatedByUserId = userId;



            await _context.SaveChangesAsync();

                return NoContent();
        }
        [HttpPost("Picture/Upload")]
        //[Authorize(Roles = "Member")]
        public async Task<string> PostImage(IFormFile pic)
        {
           
           
            if (pic == null || pic.Length == 0)
            {
                throw new ArgumentException("Invalid file. Please upload a valid image.");
            }
            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(pic.FileName)}";
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Pictures");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await pic.CopyToAsync(stream);
            }
            return fileName;
        }




        [HttpPost("profile")]
        [Authorize(Roles ="Member")]
        public async Task<IActionResult> PostProfile(UserProfileVM model, [FromHeader(Name = "Authorization")] string token)
        {
            //string userId;
            //try
            //{
            //    if (!string.IsNullOrEmpty(token))
            //    {
            //        var tokenParts = token.Split('.');
            //        if (tokenParts.Length > 1)
            //        {

            //            var payload = tokenParts[1];
            //            var base64Payload = payload.Replace('-', '+').Replace('_', '/');
            //            var jsonPayload = System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(base64Payload));


            //            var jwtPayload = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

            //            if (jwtPayload.ContainsKey("sub"))
            //            {
            //                userId = jwtPayload["sub"];
            //            }
            //            else
            //            {
            //                throw new Exception("User ID not found in payload.");
            //            }
            //        }
            //        else
            //        {
            //            throw new Exception("Invalid token structure.");
            //        }
            //    }
            //    else
            //    {
            //        return BadRequest("Token is null or empty.");
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Log exception
            //    Console.WriteLine(ex.Message);
            //    return BadRequest("Invalid token format or missing ID.");
            //}

            //var user = await _context.Users.FindAsync(userId);
            //if (user == null)
            //{
            //    return NotFound("User not found.");
            //}
            var userId = GetUserIdFromToken(token);

            var existingProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.AppUserId == userId);

            if (existingProfile == null)
            {
                var profile = new UserProfile
                {
                    FirstName = model.FirstName ?? "N/A",
                    LastName = model.LastName ?? "N/A",
                    Address = model.Address ?? "N/A",
                    DateOfBirth = model.DateOfBirth ?? default(DateTime),
                    Gender = model.Gender ?? "N/A",
                    ProfilePictureUrl = model.ProfilePictureUrl ?? "defaultpic.jpg",
                    City = model.City ?? "N/A",
                    Country = model.Country ?? "N/A",
                    AppUserId = userId,
                    CreatedByUserId = userId,
                    IsDeleted = false,
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedByUserId = userId,
                    CreatedAt = DateTime.UtcNow


                };
                await _context.UserProfiles.AddAsync(profile);
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest("User Already Has a profile");
            }
            
             

            return NoContent();
        }


        [HttpGet("all-users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = _userManager.Users;
            return Ok(await users.ToListAsync());
        }




        [HttpGet("premium-content")]
        [Authorize(Roles = "Premium-Member")]
        public IActionResult GetPremiumContent()
        {
            return Ok(new { message = "This is premium content!" });
        }

       
        [HttpGet("general-content")]
        [Authorize(Roles = "Member,Premium-Member")]
        public IActionResult GetGeneralContent()
        {
            
            return Ok(new { message = "This is general content for Members and Premium Members!" });
        }

        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                return Ok(new { Username = user.UserName, Email = user.Email });
            }
            return NotFound();
        }

        private string GenerateJwtToken(AppUser user)
        {
            // Claims
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Name, user.UserName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), 
            new Claim(JwtRegisteredClaimNames.PhoneNumber, user.PhoneNumber),
            new Claim(JwtRegisteredClaimNames.Jti, System.Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

            
            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            
            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

           
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: System.DateTime.UtcNow.AddHours(10),
                signingCredentials: creds
            );

            return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
        }


        //    private async Task<string> GenerateJwtToken(AppUser user)
        //    {
        //        // Claims
        //        var claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.NameIdentifier, user.Id), // Unique user identifier
        //    new Claim(ClaimTypes.Name, user.UserName),
        //    new Claim(ClaimTypes.Email, user.Email),
        //    new Claim("phone_number", user.PhoneNumber), // Custom claim for phone number
        //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token identifier
        //};

        //        // Adding roles to claims
        //        var roles = await _userManager.GetRolesAsync(user); // Await async method
        //        foreach (var role in roles)
        //        {
        //            claims.Add(new Claim(ClaimTypes.Role, role));
        //        }

        //        // Symmetric key and signing credentials
        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        // Token Creation
        //        var token = new JwtSecurityToken(
        //            issuer: _configuration["Jwt:Issuer"],
        //            audience: _configuration["Jwt:Audience"],
        //            claims: claims,
        //            expires: DateTime.UtcNow.AddHours(1), // Shorter token expiration
        //            signingCredentials: creds
        //        );

        //        return new JwtSecurityTokenHandler().WriteToken(token);
        //    }










        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Ok("If the email is registered, a password reset link has been sent."); // Prevents user enumeration

            // Generate the password reset token
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Create the reset link
            //var resetLink = Url.Action("ResetPassword", "Account", new { token, email = user.Email }, Request.Scheme);

            // Send the reset link to the user's email (implement your email service here)
            //await _emailService.SendAsync(user.Email, "Password Reset", $"Click here to reset your password: {resetLink}");

            //return Ok("If the email is registered, a password reset link has been sent.");

            return Ok(token);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return BadRequest("Invalid request.");

        
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password has been reset successfully.");
        }



        [HttpPut("update-password/{username}")]
        public async Task<IActionResult> ChangePassword(string username, [FromBody] ChangePasswordRequest model)
        {
    
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return BadRequest("User not found.");

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password updated successfully.");
        }


        [HttpPost("upgrade-role")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> UpgradeRole([FromBody] UpgradeRoleModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound(new { message = "User not found." });

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, model.NewRole);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = $"User role upgraded to '{model.NewRole}'." });
        }


       // Default Role Assign at the time of Registration
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest(new { message = "Email is already in use." });

            // Create user
            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            // Assign default role
            await _userManager.AddToRoleAsync(user, "Member");

            return Ok(new { message = "Registration successful. You are assigned the 'Member' role." });
        }



        //after payment role upgrade
        //public async Task<IActionResult> UpgradeToPremium(string userId)
        //{
        //    var user = await _userManager.FindByIdAsync(userId);
        //    if (user == null)
        //        return NotFound(new { message = "User not found." });

        //    var hasPaid = await _paymentService.VerifyPayment(userId);
        //    if (!hasPaid)
        //        return BadRequest(new { message = "Payment not verified. Upgrade denied." });

        //    await _userManager.AddToRoleAsync(user, "Premium");
        //    return Ok(new { message = "User upgraded to Premium role." });
        //}
        // Notify the user about the role upgrade
        //await _emailService.SendAsync(user.Email, "Role Upgrade", "Congratulations! Your membership has been upgraded.");




        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] Register model)
        //{

        //    if (await _userManager.FindByEmailAsync(model.Email) != null)
        //        return BadRequest(new { message = "Email is already in use." });


        //    var role = model.Role ?? "Member"; // Default to "Member" if no role is provided
        //    if (role != "Member" && role != "Premium-Member" && role != "Platinum")
        //        return BadRequest(new { message = "Invalid role specified." });


        //    var user = new IdentityUser { UserName = model.UserName, Email = model.Email, PhoneNumber = model.PhoneNumber };
        //    var result = await _userManager.CreateAsync(user, model.Password);

        //    if (!result.Succeeded)
        //        return BadRequest(result.Errors);

        //    await _userManager.AddToRoleAsync(user, role);

        //    return Ok(new { message = "Registration successful" });
        //}


        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] Register model)
        //{
        //    if (await _userManager.FindByEmailAsync(model.Email) != null)
        //        return BadRequest(new { message = "Email is already in use." });


        //    var user = new IdentityUser { UserName = model.UserName, Email = model.Email, PhoneNumber = model.PhoneNumber };
        //    var result = await _userManager.CreateAsync(user, model.Password);

        //    if (!result.Succeeded)
        //        return BadRequest(result.Errors);


        //    var role = model.Role ?? "Member"; // Default to "Member" if no role is provided
        //    if (role != "Member" && role != "Premium-Member" && role != "Platinum")
        //        return BadRequest(new { message = "Invalid role specified." });

        //    await _userManager.AddToRoleAsync(user, role);

        //    return Ok(new { message = "Registration successful" });
        //}


        [HttpGet("dashboard-stats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var totalUsers = await _userManager.Users.CountAsync();
            var roles = _roleManager.Roles.ToList();
            var roleCounts = new List<object>();

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                roleCounts.Add(new { Role = role.Name, Count = usersInRole.Count });
            }

            return Ok(new
            {
                TotalUsers = totalUsers,
                RoleCounts = roleCounts
            });
        }



        [HttpGet("user-with-role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserWithRole()
        {
            var users = _userManager.Users.ToList(); 
            var userRoles = new List<object>();

            foreach (var user in users)
            {
            
                var roles = await _userManager.GetRolesAsync(user);

                userRoles.Add(new
                {
                    UserId = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    Roles = roles 
                });
            }

            return Ok(userRoles);
        }


        [HttpPost("register-with-profile")]
        public async Task<IActionResult> RegisterWithProfile([FromBody] RegWithProfile model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest(new { message = "Email is already in use." });

            var user = new AppUser { UserName = model.UserName, Email = model.Email, PhoneNumber = model.PhoneNumber };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

           
            await _userManager.AddToRoleAsync(user, "Member");

            

            var userProfile = new UserProfile
            {
                AppUserId = user.Id,
                FirstName = model.FirstName ?? "No Name",
                LastName = model.LastName ?? "No Name",
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender ?? "Not Saved any Gender",
                Address = model.Address ?? "Not Saved",
                City = model.City ?? "Not Saved",
                Country = model.Country ?? "Not Saved",
                ProfilePictureUrl = model.ProfilePictureUrl ?? "defaultpic.jpg",


                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = user.Id,
                LastModifiedByUserId = user.Id,
                LastModifiedAt = DateTime.UtcNow,
                IsDeleted = false,

            };
            //await _context.Users.AddAsync(user);
             await _context.UserProfiles.AddAsync(userProfile);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registration successful. You are assigned the 'Member' role." });
        }


        //[HttpGet("user-profile/{id}")]
        //[Authorize]
        //public async Task<IActionResult> GetUserProfile(string id)
        //{
        //    var user = await _userManager.FindByIdAsync(id);
        //    if (user == null)
        //        return NotFound(new { message = "User not found." });

        //    var profile = await _context.UserProfiles.FindAsync(id);
        //    if (profile == null)
        //        return NotFound(new { message = "Profile not found." });

        //    return Ok(new
        //    {
        //        UserName = user.UserName,
        //        Email = user.Email,
        //        PhoneNumber = user.PhoneNumber,
        //        Profile = profile
        //    });
        //}
        [HttpPost("post-userProfile")]
        [Authorize]
        public async Task<IActionResult> PostProfile([FromBody] UserProfile profile)
        {
            var user = _context.Users.FirstOrDefault();
            var userProfile = new UserProfile
            {
                AppUserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
                CreatedByUserId = user.Id,
                IsDeleted = false,
             

                FirstName = profile.FirstName,
                LastName = profile.LastName,
                DateOfBirth = profile.DateOfBirth,
                Gender = profile.Gender,
                Address = profile.Address,
                City = profile.City,
                Country = profile.Country,
                ProfilePictureUrl = profile.ProfilePictureUrl
            };
            await _context.UserProfiles.AddAsync(userProfile);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
