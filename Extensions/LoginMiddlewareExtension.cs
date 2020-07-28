using Microsoft.AspNetCore.Builder;
using Inventory.Middleware;
using System;

namespace Inventory.Extensions
{
    public static class LoginMiddlewareExtension
    {
        public static IApplicationBuilder UseLoginMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoginMiddleware>();
        }
    }
}