using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Inventory.Models
{
    public class Order
    {
        public int id { get; set; }
        [ForeignKey("stock")]
        public int stockId { get; set;}
        public Stock stock { get; set;}
        public string itemName { get; set; }
        [DataType(DataType.Date)]
        public DateTime submitDate { get; set; }
        [Required()]
        public int itemAmount { get; set; }
        public enum Status {
            [Display(Name = "Unsent")]
            unsent,
            [Display(Name = "Sent")]
            sent,
            [Display(Name = "Accepted")]
            receivedByVendor,
            [Display(Name = "Shipped")]
            shipped,
            [Display(Name = "Received")]
            received,
            [Display(Name = "Completed")]
            completed,
        }
        [EnumDataType(typeof(Status))]
        public Status status { get; set; }

        [ForeignKey("user")]
        public int userId { get; set; }
        public User user;
    }
}