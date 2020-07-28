using Microsoft.AspNetCore.Builder;
using Inventory.Middleware;
using System;

namespace Inventory.Extensions
{
    public static class NoStockMiddlewareExtension
    {
        public static IApplicationBuilder UseNoStockMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NoStockMiddleware>();
        }
    }
}