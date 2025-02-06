using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RentalSystem.Models
{
    public class RentsDbContext : IdentityDbContext<AppUser>
    {
        public RentsDbContext(DbContextOptions<RentsDbContext> options) : base(options) { }

        public DbSet<UserProfile> UserProfiles { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<Post> Posts { get; set; } = default!;
        public DbSet<Contact> Contacts { get; set; } = default!;
        public DbSet<Room> Rooms { get; set; } = default!;
        public DbSet<Address> Addresses { get; set; } = default!;
        public DbSet<Facility> Facilities { get; set; } = default!;
        public DbSet<Application> Applications { get; set; }=default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>()
                .HasMany(au => au.UserProfiles)
                .WithOne(up => up.AppUser)
                .HasForeignKey(up => up.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<AppUser>()
                .HasMany(au => au.Posts)
                .WithOne(p => p.AppUser)
                .HasForeignKey(p => p.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Category>()
                .HasMany(c => c.Posts)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); 


            //modelBuilder.Entity<Post>()
            //    .HasMany(p => p.Pictures)
            //    .WithOne(pic => pic.Post)
            //    .HasForeignKey(pic => pic.PostId)
            //    .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Post>()
                .HasMany(p => p.Contacts)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Restrict); 


            modelBuilder.Entity<Post>()
                .HasMany(p => p.Rooms)
                .WithOne(rd => rd.Post)
                .HasForeignKey(rd => rd.PostId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Post>()
                .HasMany(p => p.Facilities)
                .WithOne(f => f.Post)
                .HasForeignKey(f => f.PostId)
                .OnDelete(DeleteBehavior.Restrict); 


            modelBuilder.Entity<Post>()
                .HasOne(p => p.Address)
                .WithOne(a => a.Post)
                .HasForeignKey<Address>(a => a.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Contact>()
            .HasOne(c => c.Post)
            .WithMany(p => p.Contacts)
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Facility>()
            .HasOne(f => f.Post)
            .WithMany(p => p.Facilities)
            .HasForeignKey(f => f.PostId)
            .OnDelete(DeleteBehavior.Cascade);


        }

    }
}
