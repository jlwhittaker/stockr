using System;
using System.Collections.Generic;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class StockViewModel
    {
        public IList<Stock> stocks { get; set; }
        public IList<Order> orders { get; set; }
        public bool isNew { get; set; }
        public bool sellItem { get; set; }

        public StockViewModel(IList<Stock> _stocks, IList<Order> _orders, bool _isNew, bool _sellItem)
        {
            orders = _orders;
            stocks = _stocks;
            isNew = _isNew;
            sellItem = _sellItem;

        }
    }
}