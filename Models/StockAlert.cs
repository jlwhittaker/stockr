using System;
using Inventory.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Inventory.Models {
    public class StockAlert
    {
        public int id { get; set; }
        public bool active { get; set; }
        public bool triggered { get; set; }
        public enum AlertType 
        {
            [Display(Name = "Low Stock", 
                     Description = "Quantity is below {0}", 
                     ShortName = "low-stock")]
            lowStock,
            [Display(Name = "Out of Stock", 
                     Description = "Item is out of stock",
                     ShortName = "no-stock")]
            outOfStock
        }
        public AlertType alertType { get; set; }
        public string itemName { get; set; }
        [ForeignKey("stock")]
        public int stockId { get; set; }
        public Stock stock { get; set; }
        public int? lowStockThreshold { get; set; }

        [ForeignKey("user")]
        public int userId { get; set; }
        public User user;
    }
}