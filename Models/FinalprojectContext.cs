using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Blank.Models
{
    public class FinalprojectContext : IdentityDbContext<IdentityUser>
    {
        public FinalprojectContext(DbContextOptions<FinalprojectContext> options)
            : base(options)
        {
        }

        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Task> Tasks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Order -> Dish (Many-to-One)
            builder.Entity<Order>()
                .HasOne(o => o.Dish)
                .WithMany(d => d.Orders)
                .HasForeignKey(o => o.DishId)
                .OnDelete(DeleteBehavior.Restrict); 

            // Order -> Table (Many-to-One)
            builder.Entity<Order>()
                .HasOne(o => o.Table)
                .WithMany(t => t.Orders)
                .HasForeignKey(o => o.TableId)
                .OnDelete(DeleteBehavior.Restrict);

            // Define relationships for Reservation
            builder.Entity<Reservation>()
                .HasOne(r => r.Restaurant)
                .WithMany(res => res.Reservations)
                .HasForeignKey(r => r.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Reservation>()
                .HasOne(r => r.Table)
                .WithMany(t => t.Reservations)
                .HasForeignKey(r => r.TableId)
                .OnDelete(DeleteBehavior.SetNull);


            builder.Entity<Restaurant>()
                .HasIndex(r => r.ResName)
                .IsUnique(); 
            builder.Entity<Table>()
                .Property(t => t.TName)
                .IsRequired(); 

            builder.Entity<Reservation>()
                .Property(r => r.CustomerName)
                .IsRequired();
        }

        public DbSet<UserViewModel> UserViewModel { get; set; } = default!;
    }
}
