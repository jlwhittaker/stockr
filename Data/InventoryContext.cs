using Microsoft.EntityFrameworkCore;
using Inventory.Models;

namespace Inventory.Data
{
    public class InventoryContext : DbContext
    {
        public InventoryContext (DbContextOptions<InventoryContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Item { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<StockAlert> StockAlert { get; set; }
        public DbSet<Vendor> Vendor { get; set; }
        public DbSet<User> User { get; set; }

    }
}