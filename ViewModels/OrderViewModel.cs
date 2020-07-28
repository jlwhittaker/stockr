using System;
using System.Collections.Generic;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class OrderViewModel
    {
        public IList<Order> orders { get; set; }
        public IList<string> itemNames { get; set; }
        public bool isNew { get; set; }

        public OrderViewModel(IList<Order> _orders, bool _isNew, IList<string> _itemNames)
        {
            orders = _orders;
            itemNames = _itemNames;
            isNew = _isNew;
        }
    }
}