using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Inventory.Models;
using Inventory.Data;
using Inventory.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;


namespace Inventory.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly InventoryContext _context;

        public HomeController(ILogger<HomeController> logger, InventoryContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            User user = HttpContext.Items["user"] as User;
            var alerts = from a in _context.StockAlert
                         where a.triggered == true &&
                         a.userId == user.id
                         select a;

            return View(await alerts.Include("stock").ToListAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
