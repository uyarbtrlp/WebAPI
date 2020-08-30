using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Model
{
    public class ProductContext:IdentityDbContext<AppUser,AppRole,string>
    {
        public ProductContext(DbContextOptions<ProductContext> options):base(options)
        {

        }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
               
            base.OnModelCreating(builder);
        }
    }
    public class Product
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
       
        public string UserId { get; set; }
        public string CategoryId { get; set; }
        [ForeignKey("UserId")]
        public AppUser User { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category{ get; set; }

    }
    public class Category
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public string Name { get; set; }

    }
}
