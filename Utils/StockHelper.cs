using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Inventory.Data;
using Microsoft.AspNetCore.Mvc;
using Inventory.Models;
using System.Linq;

namespace Inventory.Utils
{
    public static class StockHelper
    {
        public static async Task<int> UpdateStock(int stockId, 
                                                  int newAmount, 
                                                  InventoryContext _context)
        {
            //Update Stock amount
            var qStock = from s in _context.Stock
                    where s.id == stockId
                    select s;
            Stock stock = await qStock.SingleAsync();
            stock.count = newAmount;

            //Trigger any alerts
            var qAlert = from a in _context.StockAlert
                where a.stockId == stockId 
                select a;
            IEnumerable<StockAlert> alerts = await qAlert.ToListAsync();
            foreach (StockAlert alert in alerts)
            {
                if (alert.alertType == StockAlert.AlertType.outOfStock && 
                    newAmount == 0)
                {
                    alert.triggered = true;
                }
                else if (alert.alertType == StockAlert.AlertType.lowStock && 
                         newAmount <= alert.lowStockThreshold)
                {
                    alert.triggered = true;
                }
                else {
                    alert.triggered = false;
                }
            }
            await _context.SaveChangesAsync();
            return 1;
        }

    }
}