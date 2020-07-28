using System;

namespace Inventory.Models
{
    public class User
    {
        public int id { get; set; }
        public string userName { get; set; }
        public byte[] pwHash { get; set; }
// #nullable enable
        public string? sessionID { get; set; }

    }
}