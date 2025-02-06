using RentalSystem.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RentalSystem.ViewModels
{
    public class CategoryVM 
    {

        [Required, StringLength(50)]
        public string CategoryName { get; set; } = default!;
        //public string? AppUserId { get; set; } = default!;
    }

    public class PostVM : BaseEntity
    {
        public PostVM()
        {
           
            Rooms = new List<RoomVM>();
            Facilities = new List<FacilityVM>();
            Contacts = new List<ContactVM>();
            Address = new AddressVM();
        }

        
        public int PostId { get; set; }

       
        public int CategoryId { get; set; }


        public decimal Amount { get; set; } = default!;


        [StringLength(50)]
        public string? PaymentType { get; set; } = default!;

        [StringLength(200)]
        public string? Description { get; set; } = default!;
        public DateTime AvailableFromDate { get; set; }
      
       public string? PostType { get; set; }= default!;
        
        public string Picture { get; set; } =default!;
        public ICollection<RoomVM>? Rooms { get; set; } = new List<RoomVM>();
        public ICollection<FacilityVM>? Facilities { get; set; } = new List<FacilityVM>();
        public ICollection<ContactVM>? Contacts { get; set; } = new List<ContactVM>();
        public AddressVM? Address { get; set; } = default!;

        public string? PropertyName { get; set; } = default!;
        public string? PropertyNo { get; set; } = default!;
        public string? FloorNo { get; set; } = default!;
        public string? FlatName { get; set; } = default!;
        public string? NIDurl { get; set; } = default!;
        public string? AgreementDescription { get; set; } = default!;// only for sale

       
    }

    //public class PictureVM 
    //{
        

    //    public IFormFile PictureUrl { get; set; } = default!;

       
    //}

    public class ContactVM : BaseEntity
    {
        

        [StringLength(20)]
        public string Phone { get; set; } = default!;
        [StringLength(50)]
        public string EmailAddress { get; set; } = default!;

       
    }

    public class RoomVM : BaseEntity
    {
       
        [StringLength(200)]
        public string Description { get; set; } = default!;
        [StringLength(200)]
        public string Title { get; set; } = default!;
        public double SizeOfRoom { get; set; }
        public string SizeMethod { get; set; } = default!;

       
    }

    public class AddressVM : BaseEntity
    {
       

        [StringLength(50)]
        public string AreaName { get; set; } = default!;
        public string Section { get; set; } = default!;
        public string RoadNo { get; set; } = default!;
        public string BlockName { get; set; } = default!;

        public string CityName { get; set; } = default!;
        public string Thana { get; set; } = default!;
        public string WardNo { get; set; } = default!;

    }

    public class FacilityVM : BaseEntity
    {
       
        public string Heading { get; set; } = default!;
        public bool Value { get; set; }

        
    }


    public class ApplicationVm
    {
       
        //public string? AppliedByUserName { get; set; } = default!;
        //public DateTime AppliedAt { get; set; }
        public int PostId { get; set; } = default;
    }
}
