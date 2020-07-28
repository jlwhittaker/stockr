using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Inventory.Data;
using Inventory.Models;
using Inventory.ViewModels;
using Inventory.Utils;


// The stock view model has bool flag constructor params, _isNew and _sellItem
// These are used to toggle visibility of modals in the Stock view, the view is a single page
// The actions that use them are called from the dashboard (quick-add and quick-sell buttons)

namespace Inventory.Controllers
{
    public class StockController : Controller
    {
        private readonly InventoryContext _context;

        public StockController(InventoryContext context)
        {
            _context = context;
        }

        // GET: Stock
        public async Task<IActionResult> Index()
        {
            User user = HttpContext.Items["user"] as User;
            var stocks = from s in _context.Stock
                         where s.userId == user.id
                         select s;
            var orders = from o in _context.Order
                         where o.userId == user.id &&
                         o.status != Order.Status.completed
                         select o;
            StockViewModel model = new StockViewModel(await stocks.ToListAsync(),
                                                      await orders.ToListAsync(),
                                                      _isNew: false, _sellItem: false);
            return View("Stock", model);
        }

        // GET: Stock/Create
        public async Task<IActionResult> Create()
        {
            User user = HttpContext.Items["user"] as User;
            var stocks = from s in _context.Stock
                         where s.userId == user.id
                         select s;
            var orders = from o in _context.Order
                         where o.userId == user.id &&
                         o.status != Order.Status.completed
                         select o;
            StockViewModel model = new StockViewModel(await stocks.ToListAsync(),
                                                      await orders.ToListAsync(),
                                                      _isNew: true, _sellItem: false);
            return View("Stock", model);
        }

        // POST: Stock/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,itemName")] Stock stock)
        {
            if (ModelState.IsValid)
            {   
                User user = HttpContext.Items["user"] as User;
                stock.inStock = false;
                stock.count = 0;
                stock.user = user;
                stock.userId = user.id;
                _context.Add(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stock);
        }

        public async Task<IActionResult> Sell() 
        {
            User user = HttpContext.Items["user"] as User;
            var stocks = from s in _context.Stock
                         where s.userId == user.id
                         select s;
            var orders = from o in _context.Order
                         where o.userId == user.id &&
                         o.status != Order.Status.completed
                         select o;
            StockViewModel model = new StockViewModel(await stocks.ToListAsync(),
                                                      await orders.ToListAsync(),
                                                      _isNew: false, _sellItem: true);
            return View("Stock", model);
        }

        // POST: Stock/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Stock/Edit/{id:int}/{itemName:regex([[A-Za-z0-9 ]]+)}")]
        public async Task<IActionResult> Edit(int id, string itemName)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    User user = HttpContext.Items["user"] as User;
                    var qOrder = from o in _context.Order
                                 where o.userId == user.id &&
                                 o.stockId == id
                                 select o;
                    var qStock = from s in _context.Stock
                                 where s.userId == user.id &&
                                 s.id == id
                                 select s;
                    var qAlert = from a in _context.StockAlert
                                 where a.userId == user.id &&
                                 a.stockId == id
                                 select a;
                    IEnumerable<Order> orders = await qOrder.ToListAsync();
                    Stock stock = await qStock.SingleAsync();
                    stock.itemName = itemName;
                    StockAlert alert = await qAlert.SingleAsync();
                    alert.itemName = itemName;
                    foreach (var o in orders)
                    {
                        o.itemName = stock.itemName;
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockExists(id))
                    {
                        Console.WriteLine("foo");
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index");
        }

        // POST: Stock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stock = await _context.Stock.FindAsync(id);
            _context.Stock.Remove(stock);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockExists(int id)
        {
            return _context.Stock.Any(e => e.id == id);
        }

        // POST Stock/Sell/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Stock/Sell/{id:int}/{sellAmount:int}")]
        public async Task<IActionResult> Sell(int id, int sellAmount)
        {
            User user = HttpContext.Items["user"] as User;
            var q = from s in _context.Stock
                    where s.id == id
                    select s;
            Stock stock = q.Single();
            if (stock.userId == user.id)
            {
                int newAmount = stock.count - sellAmount;
                await StockHelper.UpdateStock(id, newAmount, _context);
            }        
            return RedirectToAction("Index");
        }
    }
}
