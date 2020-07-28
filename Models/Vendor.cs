using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Inventory.Models;
using System.Collections.Generic;


namespace Inventory.Models
{
    public class Vendor
    {
        public int id { get; set; }
        public string name { get; set; }
        public IEnumerable<Order> orders { get; set; }
        public IEnumerable<Item> items { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
    }
}