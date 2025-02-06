using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace RentalSystem.Models
{
    public class AppUser : IdentityUser
    {
       
        public AppUser()
        {
            UserProfiles = new List<UserProfile>();
            Posts = new List<Post>();
           
        }
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<UserProfile> UserProfiles { get; set; } = new List<UserProfile>();
       
    }


    public abstract class BaseEntity
    {
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedByUserId { get; set; }
        public DateTime? LastModifiedAt { get; set; } = DateTime.UtcNow;
        public string? LastModifiedByUserId { get; set; }
        public bool IsDeleted { get; set; } = false;
        [ForeignKey("AppUser")]
        public string? AppUserId { get; set; } = default!;
        public AppUser? AppUser { get; set; } = default!;
        public string? AppUserName { get; set; } = default!;
    }

    public class UserProfile : BaseEntity
    {
        [Key]
        public int UserProfileId { get; set; }
        public string? FirstName { get; set; } = default!;
        public string? LastName { get; set; } = default!;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; } = default!;
        public string? Address { get; set; } = default!;
        public string? City { get; set; } = default!;
        public string? Country { get; set; } = default!;
        public string? ProfilePictureUrl { get; set; } = default!;
    }

    public class Category : BaseEntity
    {
        public Category()
        {
            Posts = new List<Post>();
        }
        [Key]
        public int CategoryId { get; set; }

        [Required, StringLength(50)]
        public string CategoryName { get; set; } = default!;

        public ICollection<Post> Posts { get; set; } = new List<Post>();

    }

    public class Post : BaseEntity
    {
        public Post()
        {
           
            Rooms = new List<Room>();
            Facilities = new List<Facility>();
            Contacts = new List<Contact>();
            Address = new Address();
        }

        [Key]
        public int PostId { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; } = default!;

        public string Picture { get; set; } = default!;
        public ICollection<Room> Rooms { get; set; }
        public ICollection<Facility> Facilities { get; set; }
        public Address Address { get; set; } = default!;
        public ICollection<Contact> Contacts { get; set; }
        public string PostType { get; set; } = default!; //sale or rent

        [StringLength(50)]
        public string PaymentType { get; set; } = default!;

        //Agreement Duration
        public string? AgreementDescription { get; set; } = default!;// only for sale

        //for short timestays folder/path : wwwroot/NIDs
        public string? NIDurl {  get; set; }= default!; //only for short timstays 

        public decimal Amount { get; set; } = default!;

        //public string Title { get; set; } = default!; Properyname instead

        public DateTime AvailableFromDate { get; set; }
        


        [StringLength(50)]
        public string PropertyName { get; set; } = default!;
        public string? PropertyNo { get; set; } = default!;
        public string? FloorNo { get; set; } = default!;
        public string? FlatName { get; set; } = default!;
        [StringLength(200)]
        public string? Description { get; set; } = default!;

    }

    //public class Picture : BaseEntity
    //{
    //    [Key]
    //    public int PictureId { get; set; }


    //    public string PictureUrl { get; set; } = default!;

    //    [ForeignKey("Post")]
    //    public int PostId { get; set; }
    //    public Post Post { get; set; } = default!;
    //}

    public class Contact : BaseEntity
    {
        [Key]
        public int ContactId { get; set; }

        [StringLength(20)]
        public string Phone { get; set; } = default!;
        [StringLength(50)]
        public string EmailAddress { get; set; } = default!;

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; } = default!;
    }

    public class Room : BaseEntity
    {
        [Key]
        public int RoomId { get; set; }

        [StringLength(200)]
        public string Description { get; set; } = default!;
        [StringLength(200)]
        public string Title { get; set; } = default!;
        public double SizeOfRoom { get; set; }
        public string SizeMethod { get; set; } = default!;

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; } = default!;
    }

    public class Address : BaseEntity
    {
        [Key]
        public int AddressId { get; set; }

        [StringLength(50)]
        public string AreaName { get; set; } = default!;
        public string Section { get; set; } = default!;
        public string RoadNo { get; set; } = default!;
        public string BlockName { get; set; } = default!;
        
        public string CityName { get; set; } = default!;
        public string Thana { get; set; } = default!;
        public string WardNo { get; set; } = default!;

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; } = default!;
    }

    public class Facility : BaseEntity
    {
        public int FacilityId { get; set; }
        public string Heading { get; set; } = default!;
        public bool Value { get; set; }

        [ForeignKey("Post")]
        public int PostId { get; set; }
        public Post Post { get; set; } = default!;
    }



    public class Application
    {
        public int ApplicationId { get; set; }
        public string AppliedByUserName { get; set; } = default!;
        public DateTime AppliedAt { get; set; } 
        public int PostId { get; set; } = default;
    }



}