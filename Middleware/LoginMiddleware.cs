using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Threading.Tasks;
using Inventory.Models;
using Inventory.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;



namespace Inventory.Middleware
{
    public class LoginMiddleware
    {
        private readonly RequestDelegate _next;

        public LoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext _httpContext, InventoryContext _dbContext)
        {
            //Skip any checks if going to login page
            string path = _httpContext.Request.Path.Value;
            if (path == "/Login" || path == "/Login/Login")
            {
                await _next(_httpContext);
                
            }
            else
            {
                //Check for logged in user by looking for user with matching session id
                var sessionId = _httpContext.Session.Id;
                var q = from u in _dbContext.User
                        where u.sessionID == sessionId
                        select u;
                // User is not logged in
                if (!q.Any())
                {
                    _httpContext.Response.Redirect("/Login");
                }
                else
                {
                    User user = await q.SingleAsync();
                    //Add username to context for log-out icon in header
                    _httpContext.Items.Add("user", user);
                    _httpContext.Items.Add("userName", user.userName);

                    await _next(_httpContext);
                }
                //User is logged in, continue to next middleware
                
            }
        }
    }
}