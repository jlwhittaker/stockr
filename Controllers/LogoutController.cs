using System;
using Inventory.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Inventory.Data;

namespace Inventory.Controllers
{
    public class LogoutController : Controller 
    {
        private readonly InventoryContext _context;

        public LogoutController(InventoryContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Writing user.sessionID to null will cause the middleware to require new auth
            User user = HttpContext.Items["user"] as User;
            user.sessionID = null;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Login");
        }

    }
}