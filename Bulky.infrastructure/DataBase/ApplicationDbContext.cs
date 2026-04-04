using Bulky.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bulky.infrastructure.DataBase
{
    public class ApplicationDbContext:IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> products { get; set; }    
        public DbSet<Company> companies { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ShoppingCart> shoppingCarts { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<ProductImage> productImages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id=1,Name="Action",DisplayOrder=1},
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }
                );
            modelBuilder.Entity<Product>().HasData(
       new Product
       {
           Id = 1,
           Title = "Inception",
           Description = "A mind-bending thriller about dreams within dreams.",
           ISBN = "MOV001",
           Author = "Christopher Nolan",
           ListPrice = 100,
           Price = 90,
           Price50 = 85,
           Price100 = 80,
           CategoryId = 1
       },
       new Product
       {
           Id = 2,
           Title = "Interstellar",
           Description = "A journey through space and time to save humanity.",
           ISBN = "MOV002",
           Author = "Christopher Nolan",
           ListPrice = 110,
           Price = 100,
           Price50 = 95,
           Price100 = 90,
           CategoryId = 1
       },
       new Product
       {
           Id = 3,
           Title = "The Dark Knight",
           Description = "Batman faces the Joker in Gotham City.",
           ISBN = "MOV003",
           Author = "Christopher Nolan",
           ListPrice = 95,
           Price = 85,
           Price50 = 80,
           Price100 = 75,
           CategoryId=2
       },
       new Product
       {
           Id = 4,
           Title = "Avengers: Endgame",
           Description = "The Avengers assemble for the final battle.",
           ISBN = "MOV004",
           Author = "Anthony & Joe Russo",
           ListPrice = 120,
           Price = 110,
           Price50 = 105,
           Price100 = 100,
           CategoryId=2
       },
       new Product
       {
           Id = 5,
           Title = "Titanic",
           Description = "A romantic story set on the ill-fated Titanic ship.",
           ISBN = "MOV005",
           Author = "James Cameron",
           ListPrice = 90,
           Price = 80,
           Price50 = 75,
           Price100 = 70,
           CategoryId=3
       },
       new Product
       {
           Id = 6,
           Title = "The Matrix",
           Description = "A hacker discovers the shocking truth about reality.",
           ISBN = "MOV006",
           Author = "The Wachowskis",
           ListPrice = 105,
           Price = 95,
           Price50 = 90,
           Price100 = 85,
           CategoryId=3
       }
   );
            modelBuilder.Entity<Company>().HasData(
               new Company
               {
                   Id = 1,
                   Name = "TechCorp",
                   StreetAddress = "123 Main St",
                   City = "New York",
                   State = "NY",
                   PostalCode = "10001",
                   PhoneNumber = "212-555-1234"
               },
               new Company
               {
                   Id = 2,
                   Name = "Foodies Inc.",
                   StreetAddress = "456 Market Ave",
                   City = "San Francisco",
                   State = "CA",
                   PostalCode = "94103",
                   PhoneNumber = "415-555-5678"
               },
               new Company
               {
                   Id = 3,
                   Name = "HealthPlus",
                   StreetAddress = "789 Health Blvd",
                   City = "Chicago",
                   State = "IL",
                   PostalCode = "60601",
                   PhoneNumber = "312-555-9012"
               }
           );
            base.OnModelCreating(modelBuilder);
        }
    }
}
