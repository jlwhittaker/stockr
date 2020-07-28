using System;
using System.Collections.Generic;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class AlertViewModel
    {
        public IList<string> itemNames { get; set; }
        public IList<StockAlert> alerts { get; set; }
        public bool isNew { get; set; }

        public AlertViewModel(IList<StockAlert> _alerts, IList<string> _itemNames)
        {
            alerts = _alerts;
            itemNames = _itemNames;
        }
    }
}