using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inventory.Data;
using Inventory.Models;
using Inventory.ViewModels;


namespace Inventory.Controllers
{
    public class StockAlertController : Controller
    {
        private readonly InventoryContext _context;

        public StockAlertController(InventoryContext context)
        {
            _context = context;
        }

        // GET: StockAlert
        public async Task<IActionResult> Index(bool isNew=false)
        {
            User user = HttpContext.Items["user"] as User;
            var itemNames = from s in _context.Stock
                            where s.userId == user.id
                            select s.itemName;
            var alerts = from a in _context.StockAlert
                         where a.userId == user.id
                         select a;

            ViewData["isNew"] = isNew;
            AlertViewModel model =  new AlertViewModel(await alerts.Include("stock").ToListAsync(),
                                                       await itemNames.ToListAsync());
            return View("StockAlert", model);
        }

        // GET: StockAlert/New
        public IActionResult New()
        {
            return RedirectToAction("Index", "StockAlert", new { isNew=true });
        }

        // POST: StockAlert/New
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(int id, [Bind("id,itemName,alertType,lowStockThreshold")] StockAlert stockAlert)
        {
            if (id != stockAlert.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Create new alert
                    User user = HttpContext.Items["user"] as User;
                    var qStock = from s in _context.Stock 
                                where s.itemName == stockAlert.itemName &&
                                s.userId == user.id
                                select s;
                    Stock stock = await qStock.SingleAsync();
                    stockAlert.stock = stock;
                    stockAlert.active = true;
                    stockAlert.user = user;
                    stockAlert.userId = user.id;
                    
                    //Trigger alert if necessary

                    if (stockAlert.alertType == StockAlert.AlertType.outOfStock && 
                        stock.count == 0)
                    {
                        stockAlert.triggered = true;
                    }
                    else if (stockAlert.alertType == StockAlert.AlertType.lowStock && 
                             stock.count < stockAlert.lowStockThreshold)
                    {
                        stockAlert.triggered = true;
                    }
                    else
                    {
                        stockAlert.triggered = false;
                    }

                    _context.Add(stockAlert);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockAlertExists(stockAlert.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // Set alert to be active or inactive
        // POST /StockAlert/{On|Off}/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("StockAlert/{status:alpha}/{id:int}")]
        public async Task<IActionResult> updateAlertStatus(int id, string status)
        {
            User user = HttpContext.Items["user"] as User;
            bool success = false;
            var qAlert = from a in _context.StockAlert
                        where a.id == id &&
                        a.userId == user.id
                        select a;
            StockAlert alert = await qAlert.SingleAsync();
            try 
            {
                alert.active = (status == "On");
                if (alert.active) 
                // If set to active, see if it should be triggered
                {
                    // Get stock
                    var qStock = from s in _context.Stock
                                 where s.id == alert.stockId &&
                                 s.userId == user.id
                                 select s;
                    Stock stock = await qStock.SingleAsync();
                    if (alert.alertType == StockAlert.AlertType.outOfStock &&
                        stock.count == 0)
                        {
                            alert.triggered = true;
                        }
                    else if (stock.count <= alert.lowStockThreshold)
                    {
                        alert.triggered = true;
                    }
                }
                else
                // Make sure alert is no longer triggered if inactive
                {
                    alert.triggered = false;
                }
                _context.SaveChanges();
                success = true;
            }
            catch (DbUpdateConcurrencyException)
                {
                    if (!StockAlertExists(alert.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            return Json(
                new {
                    success = success,
                }
            );
        }


        // Edit alert type and lowStockThreshold
        // POST: StockAlert/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("StockAlert/Edit/{id:int}/{newType:int}/{th:int}")]
        public async Task<IActionResult> Edit(int id, int newType, int th)
        {
            // Get alert
            User user = HttpContext.Items["user"] as User;
            var qAlert = from a in _context.StockAlert  
                    where a.id == id &&
                    a.userId == user.id
                    select a;
            StockAlert alert = await qAlert.SingleAsync();
            // Get stock to check for alert trigger condition
            var qStock = from s in _context.Stock
                         where s.id == alert.stockId
                         select s;
            Stock stock = await qStock.SingleAsync();
            try
                {
                    // Update alert
                    alert.alertType = (StockAlert.AlertType)newType;
                    alert.lowStockThreshold = th;
                    // See if alert should be triggered
                    if (alert.alertType == StockAlert.AlertType.outOfStock &&
                        stock.count == 0)
                    {
                        alert.triggered = true;
                    }
                    else if (alert.alertType == StockAlert.AlertType.lowStock && 
                             stock.count <= th)
                    {
                        alert.triggered = true;
                    }
                    else
                    {
                        alert.triggered = false;    
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockAlertExists(alert.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            
            return RedirectToAction("Index");
        }

        // POST: StockAlert/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            User user = HttpContext.Items["user"] as User;
            var stockAlert = await _context.StockAlert.FindAsync(id);
            if (stockAlert.userId == user.id) 
            {
                _context.StockAlert.Remove(stockAlert);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Dismiss alert trigger, done from dashboard
        // POST: StockAlert/Dismiss/5
        [HttpPost, ActionName("Dismiss")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dismiss(int id)
        {
            bool success;
            User user = HttpContext.Items["user"] as User;
            var stockAlert = await _context.StockAlert.FindAsync(id);
            if (stockAlert.userId != user.id)
            {
                success = false;
            }
            else
            {
                stockAlert.triggered = false;
                _context.SaveChanges();
                success = true;
            }
            
            return Json(new {
                success = success
            });
        }

         private bool StockAlertExists(int id)
        {
            return _context.StockAlert.Any(e => e.id == id);
        }

    }
}
