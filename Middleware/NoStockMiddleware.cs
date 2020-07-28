using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Threading.Tasks;
using Inventory.Models;
using Inventory.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;


// If there are no items in stock, enable no-stock-modal via httpContext
namespace Inventory.Middleware
{
    public class NoStockMiddleware
    {
        private readonly RequestDelegate _next;

        public NoStockMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext _httpContext, InventoryContext _dbContext)
        {
            //Skip any checks if going to login page or stock creation
            string path = _httpContext.Request.Path.Value;
            if (path == "/Login" || path == "/Login/Login" || path == "/Stock/Create")
            {
                await _next(_httpContext);
                
            }
            else
            {
                
                //Look for stock for associated user
                User user = _httpContext.Items["user"] as User;
                var q = from s in _dbContext.Stock 
                        where s.userId == user.id
                        select s;

                // If no stock, add bool flag to context
                if (!q.Any())
                {
                    _httpContext.Items.Add("noStock", true);
                }

                await _next(_httpContext);      
            }
        }
    }
}