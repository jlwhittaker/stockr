// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Rendering;
// using Microsoft.EntityFrameworkCore;
// using Inventory.Data;
// using Inventory.Models;
// using Inventory.ViewModels;
// using Inventory.Utils;

// // OrderViewModel takes a bool flag as one of its constructor params, 
// // This is used to toggle visibility of the 'New Order' form in the view
// // You can also get to the new order form directly from an item view at /Stock/
// // (CreateQuick)
// // In this case, ViewData holds the item name to force a default <select> option in the view
// // Additionally, you can go straight to the detail modal of a specific order from the item view
// // In this case, ViewData holds the order id (see Details() GET action)

// namespace Inventory.Controllers
// {
//     public class OrderController : Controller
//     {
//         private readonly InventoryContext _context;

//         public OrderController(InventoryContext context)
//         {
//             _context = context;
//         }

//         // GET: Order
//         public async Task<IActionResult> Index()
//         {
//             User user = HttpContext.Items["user"] as User;
//             var orders = from o in _context.Order
//                          where o.userId == user.id 
//                          select o;
//             var itemNames = from s in _context.Stock
//                             where s.userId == user.id
//                             select s.itemName;             
//             OrderViewModel model = new OrderViewModel(await orders.Include("stock").ToListAsync(), 
//                                                       false,
//                                                       await itemNames.ToListAsync());
//             return View("Order", model);
//         }

//         public async Task<IActionResult> Create()
//         {
//             User user = HttpContext.Items["user"] as User;
//             var orders = from o in _context.Order
//                          where o.userId == user.id 
//                          select o;
//             var itemNames = from s in _context.Stock
//                             where s.userId == user.id
//                             select s.itemName;
//             OrderViewModel model = new OrderViewModel(await orders.Include("stock").ToListAsync(), 
//                                                       true,
//                                                       await itemNames.ToListAsync());
//             return View("Order", model);
//         }

//         // GET /Order/Create/{id}
//         [HttpGet]
//         [Route("Order/CreateQuick/{itemId:int}")]
//         public async Task<IActionResult> CreateQuick(int itemId)
//         {
//             User user = HttpContext.Items["user"] as User;
//             var orders = from o in _context.Order
//                          where o.userId == user.id
//                          select o;
//             var itemNames = from s in _context.Stock
//                             where s.userId == user.id
//                             select s.itemName;
//             var newOrderItemName = from s in _context.Stock
//                                    where s.userId == user.id &&
//                                    s.id == itemId
//                                    select s;
//             Stock stock = await newOrderItemName.SingleAsync();
//             ViewData["ItemName"] = stock.itemName;
//             OrderViewModel model = new OrderViewModel(await orders.Include("stock").ToListAsync(), 
//                                                       true,
//                                                       await itemNames.ToListAsync());
//             return View("Order", model);
//         }

//         // POST: Order/Create

//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> Create([Bind("id,itemName,itemAmount")] Order order)
//         {
//             if (ModelState.IsValid)
//             {
//                 User user = HttpContext.Items["user"] as User;
//                 var stock = from s in _context.Stock
//                             where s.userId == user.id &&
//                             s.itemName == order.itemName
//                             select s;
//                 order.stock = stock.Single();
//                 order.user = user;
//                 order.userId = user.id;
//                 order.submitDate = DateTime.Now;
//                 _context.Add(order);
//                 await _context.SaveChangesAsync();
//                 return RedirectToAction("Index");
//             }
//             return RedirectToAction("Index");
//         }

//         // GET /Order/{id}
//         [Route("Order/{id:int}")]
//         public async Task<IActionResult> Details(int id) {
//             User user = HttpContext.Items["user"] as User;
//             ViewData["OrderId"] = id;
//             var orders = from o in _context.Order
//                          where o.userId == user.id
//                          select o;
//             var itemNames = from s in _context.Stock
//                             where s.userId == user.id
//                             select s.itemName;             
//             OrderViewModel model = new OrderViewModel(await orders.Include("stock").ToListAsync(), 
//                                                       false,
//                                                       await itemNames.ToListAsync());
//             return View("Order", model);
//         }


//         // Asp.NET model binding got weird with AJAX, so it's done with url params here
//         // I'm not a huge fan of this approach, but with just a couple params I suppose it's fine
//         // Ideally, Fetch would wrap these in form data, but I kept getting 400 Bad Request,
//         // and it just didn't seem worth the hassle, 
//         [HttpPost]
//         [ValidateAntiForgeryToken]
//         [Route("Order/Update/{id:int}/{statusID:int}/{qty:int}")]
//         public async Task<IActionResult> Update(int id, int statusID, int qty)
//         {
//             User user = HttpContext.Items["user"] as User;
//             bool success = false;
//             var q = from o in _context.Order
//                         where o.userId == user.id &&
//                         o.id == id
//                         select o;
//             Order order = await q.Include("stock").SingleAsync();
//             if (statusID != (int)order.status) 
//             {
//                 try
//                 {
//                     order.status = (Order.Status)statusID;
//                     if (statusID == 5)
//                     {
//                         int newStockAmount = order.stock.count + order.itemAmount;
//                         await StockHelper.UpdateStock(order.stockId, newStockAmount, _context);
//                     }
//                     await _context.SaveChangesAsync();
//                     success = true;
//                 }
//                 catch (DbUpdateConcurrencyException)
//                 {
//                     if (!OrderExists(order.id))
//                     {
//                         return NotFound();
//                     }
//                     else
//                     {
//                         throw;
//                     }
//                 }
//             }
//             return Json(
//                 new {
//                     success = success
//                 }
//             );
//         }

//         // POST: Order/Delete/5
//         [HttpPost, ActionName("Delete")]
//         [ValidateAntiForgeryToken]
//         public async Task<IActionResult> DeleteConfirmed(int id)
//         {
//             bool success = false;
//             User user = HttpContext.Items["user"] as User;
//             var order = await _context.Order.FindAsync(id);
//             if (order.userId == user.id)
//             {
//                 _context.Order.Remove(order);
//                 await _context.SaveChangesAsync();
//                 success = true;
//             }
            
//             return Json(
//                 new {
//                     success = success
//                 }
//             );
//         }

//         private bool OrderExists(int id)
//         {
//             return _context.Order.Any(e => e.id == id);
//         }
//     }
// }


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
using Inventory.Utils;

// OrderViewModel takes a bool flag as one of its constructor params, 
// This is used to toggle visibility of the 'New Order' form in the view
// You can also get to the new order form directly from an item view at /Stock/
// (CreateQuick)
// In this case, ViewData holds the item name to force a default <select> option in the view
// Additionally, you can go straight to the detail modal of a specific order from the item view
// In this case, ViewData holds the order id (see Details() GET action)

namespace Inventory.Controllers
{
    public class OrderController : Controller
    {
        private readonly InventoryContext _context;

        public OrderController(InventoryContext context)
        {
            _context = context;
        }

        // GET: Order
        public async Task<IActionResult> Index()
        {
            User user = HttpContext.Items["user"] as User;
            var orders = from o in _context.Order
                         where o.userId == user.id 
                         select o;
            var itemNames = from s in _context.Stock
                            where s.userId == user.id
                            select s.itemName;             
            OrderViewModel model = new OrderViewModel(await orders.Include("stock").ToListAsync(), 
                                                      false,
                                                      await itemNames.ToListAsync());
            return View("Order", model);
        }

        public async Task<IActionResult> Create()
        {
            User user = HttpContext.Items["user"] as User;
            var orders = from o in _context.Order
                         where o.userId == user.id 
                         select o;
            var itemNames = from s in _context.Stock
                            where s.userId == user.id
                            select s.itemName;
            OrderViewModel model = new OrderViewModel(await orders.Include("stock").ToListAsync(), 
                                                      true,
                                                      await itemNames.ToListAsync());
            return View("Order", model);
        }

        // GET /Order/Create/{id}
        [HttpGet]
        [Route("Order/CreateQuick/{itemId:int}")]
        public async Task<IActionResult> CreateQuick(int itemId)
        {
            User user = HttpContext.Items["user"] as User;
            var orders = from o in _context.Order
                         where o.userId == user.id
                         select o;
            var itemNames = from s in _context.Stock
                            where s.userId == user.id
                            select s.itemName;
            var newOrderItemName = from s in _context.Stock
                                   where s.userId == user.id &&
                                   s.id == itemId
                                   select s;
            Stock stock = await newOrderItemName.SingleAsync();
            ViewData["ItemName"] = stock.itemName;
            OrderViewModel model = new OrderViewModel(await orders.Include("stock").ToListAsync(), 
                                                      true,
                                                      await itemNames.ToListAsync());
            return View("Order", model);
        }

        // POST: Order/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,itemName,itemAmount")] Order order)
        {
            if (ModelState.IsValid)
            {
                User user = HttpContext.Items["user"] as User;
                var stock = from s in _context.Stock
                            where s.userId == user.id &&
                            s.itemName == order.itemName
                            select s;
                order.stock = stock.Single();
                order.user = user;
                order.userId = user.id;
                order.submitDate = DateTime.Now;
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        // GET /Order/{id}
        [Route("Order/{id:int}")]
        public async Task<IActionResult> Details(int id) {
            User user = HttpContext.Items["user"] as User;
            ViewData["OrderId"] = id;
            var orders = from o in _context.Order
                         where o.userId == user.id
                         select o;
            var itemNames = from s in _context.Stock
                            where s.userId == user.id
                            select s.itemName;             
            OrderViewModel model = new OrderViewModel(await orders.Include("stock").ToListAsync(), 
                                                      false,
                                                      await itemNames.ToListAsync());
            return View("Order", model);
        }


        // Asp.NET model binding got weird with AJAX, so it's done with url params here
        // I'm not a huge fan of this approach, but with just a couple params I suppose it's fine
        // Ideally, Fetch would wrap these in form data, but I kept getting 400 Bad Request,
        // and it just didn't seem worth the hassle, 
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Order/Update/{id:int}/{statusID:int}/{qty:int}")]
        public async Task<IActionResult> Update(int id, int statusID, int qty)
        {
            User user = HttpContext.Items["user"] as User;
            bool success = false;
            var q = from o in _context.Order
                        where o.userId == user.id &&
                        o.id == id
                        select o;
            Order order = await q.Include("stock").SingleAsync();
            if (statusID != (int)order.status) 
            {
                try
                {
                    order.status = (Order.Status)statusID;
                    if (statusID == 5)
                    {
                        int newStockAmount = order.stock.count + order.itemAmount;
                        await StockHelper.UpdateStock(order.stockId, newStockAmount, _context);
                    }
                    await _context.SaveChangesAsync();
                    success = true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return Json(
                new {
                    success = success
                }
            );
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool success = false;
            User user = HttpContext.Items["user"] as User;
            var order = await _context.Order.FindAsync(id);
            if (order.userId == user.id)
            {
                _context.Order.Remove(order);
                await _context.SaveChangesAsync();
                success = true;
            }
            
            return Json(
                new {
                    success = success
                }
            );
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.id == id);
        }
    }
}
