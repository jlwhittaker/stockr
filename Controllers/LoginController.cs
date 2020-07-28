using System;
using System.Collections.Generic;
using System.Security.Cryptography;
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
using System.Text;

namespace Inventory.Controllers
{
    public class LoginController : Controller {
        private readonly InventoryContext _context;

        public LoginController(InventoryContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View("Login");
        }

        // Auth works as follows:
        // Session is established in this controller (see Session.SetString("init", "0") below)
        // and session ID is written to user.sessionID in the DB.
        // Middleware checks the session ID of every request, and finds a matching user.
        // If there is no session ID, or the user isn't found, then the user is not logged in
        // And middleware redirects to /Login

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password, string loginChoice)
        {
            User user;
            byte[] pwHash;

            // Attempt to create new user
            if (loginChoice == "newUser")
            {
                // Make sure username is not taken
                if (_context.User.Any(u => u.userName == userName))
                {
                    ViewData["error"] = "Username Taken";
                    return View("Login");
                }
                // Manually start session so it doesn't rewrite with new request
                HttpContext.Session.SetString("init", "0");

                // Create user
                pwHash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
                user = new User() {
                    userName = userName,
                    pwHash = pwHash,
                    sessionID = HttpContext.Session.Id
                };
                _context.Add(user);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("Index", "Home");
            }

            // Attempt to log in 
            if (!_context.User.Any(u => u.userName == userName))
            {
                ViewData["error"] = "Username Not Found";
                return View("Login");
            }

            pwHash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
            var qUser = from u in _context.User
                        where u.userName == userName &&
                              u.pwHash == pwHash
                        select u;
            //PW is invalid
            if (!qUser.Any())
            {
                ViewData["error"] = "Invalid Password";
                return View("Login");
            }

            // Credentials match, set user.sessionID and redirect to home
            // Middleware takes it from here

            // Manually start session as above
            HttpContext.Session.SetString("init", "0");
            user = await qUser.SingleAsync();
            user.sessionID = HttpContext.Session.Id;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");

        }
    }
}