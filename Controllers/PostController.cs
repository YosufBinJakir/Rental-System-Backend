//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using RentalSystem.Models;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace RentalSystem.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class PostController : ControllerBase
//    {
//        private static readonly List<Post> Posts = new();

//        // Get all posts (accessible to all roles)
//        [HttpGet]
//        [Authorize(Roles = "Member,Premium-Member,Admin")]
//        public IActionResult GetAllPosts()
//        {
//            return Ok(Posts);
//        }

//        // Create a new post (Members and above)
//        [HttpPost]
//        [Authorize(Roles = "Member,Premium-Member,Admin")]
//        public IActionResult CreatePost([FromBody] Post model)
//        {
//            if (model == null)
//            {
//                return BadRequest(new { message = "Invalid post data." });
//            }

//            model.Id = Posts.Count + 1; // Assign a new ID
//            Posts.Add(model);

//            return Ok(new { message = "Post created successfully.", post = model });
//        }

//        // Edit an existing post (Premium-Member and Admin only)
//        [HttpPut("{id}")]
//        [Authorize(Roles = "Premium-Member,Admin")]
//        public IActionResult EditPost(int id, [FromBody] Post updatedPost)
//        {
//            var post = Posts.FirstOrDefault(p => p.Id == id);
//            if (post == null)
//            {
//                return NotFound(new { message = "Post not found." });
//            }

//            post.Title = updatedPost.Title;
//            post.Content = updatedPost.Content;

//            return Ok(new { message = "Post updated successfully.", post });
//        }

//        // Delete a post (Admin only)
//        [HttpDelete("{id}")]
//        [Authorize(Roles = "Admin")]
//        public IActionResult DeletePost(int id)
//        {
//            var post = Posts.FirstOrDefault(p => p.Id == id);
//            if (post == null)
//            {
//                return NotFound(new { message = "Post not found." });
//            }

//            Posts.Remove(post);
//            return Ok(new { message = "Post deleted successfully." });
//        }

//        // View a specific post (Accessible to all roles)
//        [HttpGet("{id}")]
//        [Authorize(Roles = "Member,Premium-Member,Admin")]
//        public IActionResult GetPostById(int id)
//        {
//            var post = Posts.FirstOrDefault(p => p.Id == id);
//            if (post == null)
//            {
//                return NotFound(new { message = "Post not found." });
//            }

//            return Ok(post);
//        }
//    }

//    // Example Post Model
//    public class Post
//    {
//        public int Id { get; set; }
//        public string Title { get; set; }
//        public string Content { get; set; }
//    }
//}
