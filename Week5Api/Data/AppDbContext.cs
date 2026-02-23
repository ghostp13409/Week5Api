using Microsoft.EntityFrameworkCore;
using Week5Api.Models;

namespace Week5Api.Data
{
    public class AppDbContext: DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Seed initial data
            modelBuilder.Entity<Item>().HasData(
                new Item { Id = 1, Name = "Item 1", Quantity = 10 },
                new Item { Id = 2, Name = "Item 2", Quantity = 20 },
                new Item { Id = 3, Name = "Item 3", Quantity = 30 }
            );
        }

    }
}
