using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalSystem.Models;
using RentalSystem.ViewModels;
using System;
using System.Security.Claims;

namespace RentalSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : BaseController
    {
        RentsDbContext db;
        IWebHostEnvironment env;
        public PostsController(RentsDbContext db, IWebHostEnvironment env)
        {
            this.db = db;
            this.env = env;
        }



        [HttpPost("post-category")]/// for admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PostCategory([FromBody] CategoryVM categoryInput)
        {
            var user = db.Users.FirstOrDefault();
            var category = new Category
            {
                CategoryName = categoryInput.CategoryName,
                AppUserId = user.Id
            };
            await db.Categories.AddAsync(category);
            await db.SaveChangesAsync();
            return NoContent();
        }


        [HttpPost]
        [Authorize(Roles ="Member")]
        public async Task<IActionResult> PostProperty(PostVM postInput, [FromHeader(Name = "Authorization") ] string token)
        {
            //if (files == null || files.Count == 0)
            //{
            //    return BadRequest(new { message = "No files were provided." });
            //}

            //var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PropertyImages");
            //if (!Directory.Exists(uploadPath))
            //{
            //    Directory.CreateDirectory(uploadPath);
            //}

            //var pictureUrls = new List<string>();

            //foreach (var file in files)
            //{
            //    if (file.Length > 0)
            //    {
            //        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            //        var filePath = Path.Combine(uploadPath, fileName);

            //        using (var stream = new FileStream(filePath, FileMode.Create))
            //        {
            //            await file.CopyToAsync(stream);
            //        }

            //        pictureUrls.Add(fileName);  
            //    }
            //}


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
            var post = new Post
            {
                CategoryId = postInput.CategoryId,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
                CreatedByUserId = claims.Id,
                IsDeleted = false,
                AppUserId = claims.Id,
                AppUserName = claims.UserName,
                LastModifiedByUserId = claims.Id,
               


                PaymentType = postInput.PaymentType,
               AvailableFromDate = postInput.AvailableFromDate,
                Description = postInput.Description,
                Amount = postInput.Amount,
                PropertyName = postInput.PropertyName,
                PostType = postInput.PostType,
                 PropertyNo = postInput.PropertyNo,
                FloorNo = postInput.FloorNo,
                FlatName = postInput.FlatName,
                Picture = postInput.Picture
            };



            //foreach (var picture in postInput.Pictures)
            //{
              
            //    if (picture != null)
            //    {
            
            //        string ext = Path.GetExtension(picture.FileName);

             
            //        string imageUrl = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;

         
            //        string imagePath = Path.Combine(env.WebRootPath, "Pictures", imageUrl);


                    


            //        using (var fs = new FileStream(imagePath, FileMode.Create))
            //        {
            //            await picture.CopyToAsync(fs);
            //        }


            //        //post.Pictures.Add(new Picture
            //        //{
            //        //    PostId = post.PostId,
            //        //    CreatedAt = DateTime.UtcNow,
            //        //    LastModifiedAt = DateTime.UtcNow,
            //        //    CreatedByUserId = claims.Id,
            //        //    IsDeleted = false,
            //        //    AppUserId = claims.Id,
            //        //    PictureUrl = imageUrl  
            //        //});
            //    }
            //}










            //foreach (var url in pictureUrls)
            //{
            //    post.Pictures.Add(new Picture
            //    {
            //        PostId = post.PostId,
            //        CreatedAt = DateTime.UtcNow,
            //        LastModifiedAt = DateTime.UtcNow,
            //        CreatedByUserId = claims.Id,
            //        IsDeleted = false,
            //        AppUserId = claims.Id,

            //        PictureUrl = url

            //    });

            //}

            //foreach (var p in postInput.Pictures)
            //{
            //    post.Pictures.Add(new Picture
            //    {
            //        PostId = post.PostId,
            //        CreatedAt = DateTime.UtcNow,
            //        LastModifiedAt = DateTime.UtcNow,
            //        CreatedByUserId = claims.Id,
            //        IsDeleted = false,
            //        AppUserId = claims.Id,

            //        PictureUrl = p.PictureUrl

            //    });
            //}

            foreach (var r in postInput.Rooms)
            {
                post.Rooms.Add(new Room
                {
                    PostId = post.PostId,
                    CreatedAt = DateTime.UtcNow,
                    LastModifiedAt = DateTime.UtcNow,
                    CreatedByUserId = claims.Id,
                    IsDeleted = false,


                    Description = r.Description,
                    SizeOfRoom = r.SizeOfRoom,
                    SizeMethod = r.SizeMethod,
                    Title = r.Title


                });
            }


            foreach (var f in postInput.Facilities)
            {
                post.Facilities.Add(new Facility
                {
                    PostId = post.PostId,
                    CreatedAt = DateTime.UtcNow,
                    LastModifiedAt = DateTime.UtcNow,
                    CreatedByUserId = claims.Id,
                    IsDeleted = false,


                    Heading = f.Heading,
                    Value = f.Value



                });
            }


            foreach (var c in postInput.Contacts)
            {
                post.Contacts.Add(new Contact
                {
                    PostId = post.PostId,
                    CreatedAt = DateTime.UtcNow,
                    LastModifiedAt = DateTime.UtcNow,
                    CreatedByUserId = claims.Id,
                    IsDeleted = false,


                    Phone = c.Phone,
                    EmailAddress = c.EmailAddress


                });
            }

            Address ad = new Address
            {

                PostId = post.PostId,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
                CreatedByUserId = claims.Id,
                IsDeleted = false,


                CityName = postInput.Address.CityName,             
                RoadNo = postInput.Address.RoadNo,
                BlockName = postInput.Address.BlockName,
                AreaName = postInput.Address.AreaName,
                Section = postInput.Address.Section,
                Thana = postInput.Address.Thana,
                WardNo = postInput.Address.WardNo


            };

            post.Address = ad;

            await db.Posts.AddAsync(post);
            await db.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("upload/pictures")]
        public async Task<ActionResult<List<string>>> UploadPictures([FromForm] ICollection<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest(new { message = "No files were provided." });
            }

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PropertyImages");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var pictureUrls = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    pictureUrls.Add(fileName);  
                }
            }

            return Ok(new { pictures = pictureUrls });
        }



        [HttpPost("Picture/Upload")]
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


        [HttpGet]
        public async Task<IActionResult> GetPosts()
        {
            var data = await db.Posts.Include(x => x.Address).Include(x => x.Contacts).ToListAsync();
            return Ok(data);
        }

        [HttpGet("Rent-Posts")]
        public async Task<IActionResult> GetRentPosts()
        {
            var data = await db.Posts
                
                .Include(x => x.Address)
                .Include(x => x.Contacts)
                .Where(p => p.PostType == "Rent")
                .ToListAsync();

            if (!data.Any())
                return NotFound("No rent posts found.");

            return Ok(data);
        }

        [HttpGet("flat-Posts")]
        public async Task<IActionResult> GetFlatPosts()
        {
            var data = await db.Posts
                
                .Include(x => x.Address)
                .Include(x => x.Contacts)
                .Where(p => p.Category.CategoryName == "Flat" && EF.Functions.Like(p.PostType, "Rent"))
                .ToListAsync();

            if (!data.Any())
                return NotFound("No rent posts found.");

            return Ok(data);
        }


        [HttpGet("Room-Posts")]
        public async Task<IActionResult> GetRoomPosts()
        {
            var data = await db.Posts
               
                .Include(x => x.Address)
                .Include(x => x.Contacts)
                .Where(p => p.Category.CategoryName == "Rooms" && EF.Functions.Like(p.PostType, "Rent"))
                .ToListAsync();

            if (!data.Any())
                return NotFound("No rent posts found.");

            return Ok(data);
        }


        [HttpGet("Sublet-Posts")]
        public async Task<IActionResult> GetSubletPosts()
        {
            var data = await db.Posts
                
                .Include(x => x.Address)
                .Include(x => x.Contacts)
                .Where(p => p.Category.CategoryName == "Sublet" && EF.Functions.Like(p.PostType, "Rent"))
                .ToListAsync();

            if (!data.Any())
                return NotFound("No rent posts found.");

            return Ok(data);
        }

        [HttpGet("Bachelorb-Posts")]
        public async Task<IActionResult> GetBachelorBPosts()
        {
            var data = await db.Posts
                
                .Include(x => x.Address)
                .Include(x => x.Contacts)
                .Where(p => p.Category.CategoryName == "Bachelor(Boys)" && EF.Functions.Like(p.PostType, "Rent"))
                .ToListAsync();

            if (!data.Any())
                return NotFound("No rent posts found.");

            return Ok(data);
        }



        [HttpGet("Bachelorg-Posts")]
        public async Task<IActionResult> GetBachelorGPosts()
        {
            var data = await db.Posts
                
                .Include(x => x.Address)
                .Include(x => x.Contacts)
                .Where(p => p.Category.CategoryName == "Bachelor(Girls)" && EF.Functions.Like(p.PostType, "Rent"))
                .ToListAsync();

            if (!data.Any())
                return NotFound("No rent posts found.");

            return Ok(data);
        }



        [HttpGet("Shop-Posts")]
        public async Task<IActionResult> GetShopPosts()
        {
            var data = await db.Posts
                
                .Include(x => x.Address)
                .Include(x => x.Contacts)
                .Where(p => p.Category.CategoryName == "Shop" && EF.Functions.Like(p.PostType, "Rent"))
                .ToListAsync();

            if (!data.Any())
                return NotFound("No rent posts found.");

            return Ok(data);
        }


        [HttpGet("Office-Posts")]
        public async Task<IActionResult> GetOfficePosts()
        {
            var data = await db.Posts
               
                .Include(x => x.Address)
                .Include(x => x.Contacts)
                .Where(p => p.Category.CategoryName == "Office" && EF.Functions.Like(p.PostType, "Rent"))
                .ToListAsync();

            if (!data.Any())
                return NotFound("No rent posts found.");

            return Ok(data);
        }


        //----------------------Start Sale Section------------------


        [HttpGet("Sale-Posts")]
        public async Task<IActionResult> GetSalePosts()
        {
            var data = await db.Posts
                
                .Include(x => x.Address)
                .Include(x => x.Contacts)
                .Where(p => p.PostType == "Sale")
                .ToListAsync();

            if (!data.Any())
                return NotFound("No rent posts found.");

            return Ok(data);
        }

        [HttpGet("Sale-Flat-Posts")]
        public async Task<IActionResult> GetSaleFlatPosts()
        {
            var data = await db.Posts
               
                .Include(x => x.Address)
                .Include(x => x.Contacts)
                .Where(p => p.Category.CategoryName == "Flat" && EF.Functions.Like(p.PostType, "Sale"))
                .ToListAsync();

            if (!data.Any())
                return NotFound("No rent posts found.");

            return Ok(data);
        }


        [HttpGet("Sale-House-Posts")]
        public async Task<IActionResult> GetSaleHousePosts()
        {
            var data = await db.Posts
                
                .Include(x => x.Address)
                .Include(x => x.Contacts)
                .Where(p => p.Category.CategoryName == "House"  && EF.Functions.Like(p.PostType, "Sale"))
                .ToListAsync();

            if (!data.Any())
                return NotFound("No rent posts found.");

            return Ok(data);
        }


        [HttpGet("Sale-Plot-Posts")]
        public async Task<IActionResult> GetSalePlotPosts()
        {
            var data = await db.Posts
               
                .Include(x => x.Address)
                .Include(x => x.Contacts)
                .Where(p => p.Category.CategoryName == "Plot" && EF.Functions.Like(p.PostType, "Sale"))
                .ToListAsync();

            if (!data.Any())
                return NotFound("No rent posts found.");

            return Ok(data);
        }

        [HttpGet("Sale-Land-Posts")]
        public async Task<IActionResult> GetSaleLandPosts()
        {
            var data = await db.Posts
                
                .Include(x => x.Address)
                .Include(x => x.Contacts)
                .Where(p => p.Category.CategoryName == "Land" && EF.Functions.Like(p.PostType, "Sale"))
                .ToListAsync();

            if (!data.Any())
                return NotFound("No rent posts found.");

            return Ok(data);
        }



        [HttpGet("Sale-Office-Posts")]
        public async Task<IActionResult> GetSaleOfficePosts()
        {
            var data = await db.Posts
                
                .Include(x => x.Address)
                .Include(x => x.Contacts)
                .Where(p => p.Category.CategoryName == "Office" && EF.Functions.Like(p.PostType, "Sale"))
                .ToListAsync();

            if (!data.Any())
                return NotFound("No rent posts found.");

            return Ok(data);
        }



        [HttpGet("Sale-Shop-Posts")]
        public async Task<IActionResult> GetSaleShopPosts()
        {
            var data = await db.Posts
                
                .Include(x => x.Address)
                .Include(x => x.Contacts)
                .Where(p => p.Category.CategoryName == "Shop" && EF.Functions.Like(p.PostType, "Sale"))
                .ToListAsync();

            if (!data.Any())
                return NotFound("No rent posts found.");

            return Ok(data);
        }


       

        //----------------------End Sale Section---------------------
        [HttpGet("Facilities/{id}")]

        public async Task<IActionResult> GetFacilities(int id)
        {
            var data = await db.Facilities.Where(x => x.PostId == id).ToListAsync();
            return Ok(data);
        }


        [HttpGet("Get-Posts/{id}")]
        [Authorize(Roles ="Admin, Member")]
        public async Task<IActionResult> GetPostsById(int id)
        {
            var data = await db.Posts.Include(x => x.Address).Include(x => x.Rooms).Include(x => x.Contacts).Include(x => x.Facilities).FirstOrDefaultAsync(x => x.PostId == id);
            if (data == null) return NotFound();
            return Ok(data);
        }


        [HttpGet("category-list")]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> GetCategoryList()
        {
            var categories = await db.Categories.ToListAsync();
            return Ok(categories);
        }



        [HttpGet("Get-Posts-WithUsers/{id}")]
        public async Task<IActionResult> GetPostsWithUserName(int id)
        {
            var data = await db.Posts
     .Include(x => x.AppUser)
     .Select(x => new
     {

         PropertyName = x.PropertyName,
         UserName = x.AppUser.UserName,
         UserEmail = x.AppUser.Email,
        
         Address = x.Address,
         Contacts = x.Contacts.ToList(),
         Facilities = x.Facilities.ToList()
     })
     .ToListAsync();

            if (data == null || !data.Any()) return NotFound();
            return Ok(data);
        }


        [HttpGet("Get-Posts-WithUsers-sales")]
        public async Task<IActionResult> GetSaleWithoutUserPosts()
        {
            var data = await db.Posts
     .Include(x => x.AppUser).Where(x => x.PostType == "Sale")
     .Select(x => new
     {

         PropertyName = x.PropertyName,
         UserName = x.AppUser.UserName,
         UserEmail = x.AppUser.Email,
         
         Address = x.Address,
         Contacts = x.Contacts.ToList(),
         Facilities = x.Facilities.ToList()
     }).ToListAsync();

            if (data == null || !data.Any()) return NotFound();
            return Ok(data);
        }


        [HttpGet("Get-Posts-WithUsers-Rents")]
        public async Task<IActionResult> GetRentsPosts()
        {
            var data = await db.Posts
     .Include(x => x.AppUser).Where(x => x.PostType == "Rent")
     .Select(x => new
     {

         pr = x.PropertyName,
         un = x.AppUser.UserName,
         ue = x.AppUser.Email,
         
         a = x.Address,
         c = x.Contacts.ToList(),
         f = x.Facilities.ToList()
     }).ToListAsync();

            if (data == null || !data.Any()) return NotFound();
            return Ok(data);
        }

        [HttpGet("user/posts/{id}")]
        [Authorize(Roles = "Admin, Member")]
        public async Task<IActionResult> GetPostwithUserId(string id)
        {
            //if (string.IsNullOrWhiteSpace(token) || !token.StartsWith("Bearer "))
            //{
            //    return BadRequest("Invalid or missing token.");
            //}


            //var jwtToken = token.Substring("Bearer ".Length).Trim();


            //var claims = GetUserIdFromToken(jwtToken);
            //if (string.IsNullOrEmpty(claims.Id))
            //{
            //    return Unauthorized("User ID not found in token.");
            //}

            var data = await db.Posts.Where(x => x.AppUserId == id).ToListAsync();
            if (data == null)
            {
                
                return NotFound("No post yet");
            }
            
            return Ok(data);

        }




        [HttpDelete("{id}")]
        [Authorize(Roles ="Admin, Member")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var data = await db.Posts.FirstOrDefaultAsync(x => x.PostId == id);
            if (data == null) return NotFound();

            db.Posts.Remove(data);
            await db.SaveChangesAsync();
            return NoContent();

        }

        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin, Member")]
        public async Task<IActionResult> GetUserPosts([FromHeader(Name = "Authorization")] string token)
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

            var posts = await db.Posts
                                     .Where(p => p.AppUserId == claims.Id)
                                     .ToListAsync();
            if (posts == null) return NotFound("You have not any posted yet....");
            return Ok(posts);
        }




        [HttpGet("Search/Results")]
        public IActionResult Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Ok(new List<string>());

            var results = db.Posts
                .Where(r => r.PaymentType.Contains(query, System.StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(results);
        }







        [HttpPost("apply")]
        [Authorize(Roles ="Member")]
        public async Task<IActionResult> Apply([FromBody] ApplicationVm req, [FromHeader(Name = "Authorization")] string token)
        {
            if (req == null || req.PostId == 0)
            {
                return BadRequest("Invalid postId.");
            }


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


            try
            {
                var application = new Application
                {
                    AppliedByUserName = claims.UserName,
                    PostId = req.PostId,
                    AppliedAt = DateTime.UtcNow
                };

                db.Applications.Add(application);
                await db.SaveChangesAsync();

                return Ok("Successfully applied for the post.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        //[HttpDelete]
        //[Authorize(Roles ="Member, Admin")]
        //public  async Task<IActionResult> DeletePosts(int id)
        //{
        //    var post = await db.Posts.FirstOrDefaultAsync(x => x.PostId == id);
        //    if (post == null) return NotFound();
        //    db.Posts.Remove(post);

        //    return NoContent();
        //}

        //[HttpGet("Include/Property")]
        //public async Task<IActionResult> GetCategoriesAsyncWithProperty()
        //{
        //    if (db == null)
        //    {
        //        return Problem("Database context is not available.");
        //    }

        //    var categories = await db.Categories.Include(c => c.Posts).ToListAsync();
        //    if (categories == null || !categories.Any())
        //    {
        //        return NotFound("No categories found.");
        //    }

        //    return Ok(categories);
        //}
    }
}