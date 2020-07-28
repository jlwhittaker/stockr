using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Inventory.Models;
using System.Collections.Generic;


namespace Inventory.Models
{
    public class Stock
    {
        public int id { get; set; }
        public string itemName { get; set; }
        public bool inStock { get; set; }
        public int count { get; set; }

        [ForeignKey("user")]
        public int userId { get; set; }
        public User user;

    }
}