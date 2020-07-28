using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Inventory.Models;

namespace Inventory.Models
{
    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }

    }
}