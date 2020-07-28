using System;
using System.Collections.Generic;
using Inventory.Models;

namespace Inventory.ViewModels
{
    public class DashboardViewModel
    {
        public IList<StockAlert> alerts { get; set; }

        public DashboardViewModel(IList<StockAlert> _alerts)
        {
            alerts = _alerts;
        }
    }
}