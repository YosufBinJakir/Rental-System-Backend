using RentalSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace RentalSystem.ViewModels
{
    public class Register
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [MinLength(6)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
    }


    //public class Register
    //{
    //    public string UserName { get; set; }
    //    public string Email { get; set; }
    //    public string PhoneNumber { get; set; }
    //    public string Password { get; set; }
    //    public string Role { get; set; } 
    //}

    public class RegisterProfile
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public string Role { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } 
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string ProfilePictureUrl { get; set; }
    }

    public class UserProfileVM:BaseEntity
    {
        //[Required]
        //public string Email { get; set; }

        //[Required]
        //[MinLength(6)]
        //public string Password { get; set; }

        //[Required]
        //[MinLength(6)]
        //[Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        //public string ConfirmPassword { get; set; }
        //public string UserName { get; set; }
        //public string PhoneNumber { get; set; }


        public string? FirstName { get; set; } = default!;
        public string? LastName { get; set; } = default!;
        public DateTime? DateOfBirth { get; set; } 
        public string? Gender { get; set; } = default!;
        public string? Address { get; set; } = default!;
        public string? City { get; set; } = default!;
        public string? Country { get; set; } = default!;
        public string? ProfilePictureUrl { get; set; } = default!;
    }




    public class RegWithProfile : BaseEntity
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [MinLength(6)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        public string ConfirmPassword { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }


        public string? FirstName { get; set; } = "No Name";
        public string? LastName { get; set; } = " No Name";
        public DateTime? DateOfBirth { get; set; } = null;
        public string? Gender { get; set; } = "Not Saved any Gender";
        public string? Address { get; set; } = "Not Saved";
        public string? City { get; set; } = "Not Saved";
        public string? Country { get; set; } = "Not Saved";
        public string? ProfilePictureUrl { get; set; } = "defaultpic.jpg";
    }



    public class Login
    {
       
        public string? Email { get; set; }
        public string? UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    
    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }


   
    public class UpgradeRoleModel
    {
        public string UserId { get; set; }
        public string NewRole { get; set; } 
    }

    public class JwtClaims
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string PhoneNumber { get; set; }
    }

}
