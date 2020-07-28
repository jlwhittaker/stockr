using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Inventory.Data;
using System.Security.Cryptography;
using System;
using System.Text;
using System.Linq;

namespace Inventory.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new InventoryContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<InventoryContext>>()))
                    {
                        if (context.Stock.Any())
                        {
                            return;
                        }


                        User seedUser = new User() {
                            userName = "test",
                            pwHash = SHA256.Create().ComputeHash(
                                Encoding.UTF8.GetBytes("pw")
                                )
                        };
                        context.User.Add(seedUser);
                        context.SaveChanges();

                        Stock newStock1 = new Stock
                        {
                            itemName = "Garfield Ceramic Mug, Large",
                            inStock = true,
                            count = 13,
                            user = seedUser,
                            userId = seedUser.id
                        };
                        Stock newStock2 = new Stock
                        {
                            itemName = "Sunflower Dish Towel",
                            inStock = true,
                            count = 9,
                            user = seedUser,
                            userId = seedUser.id

                        };
                        Stock newStock3 = new Stock
                        {
                            itemName = "Arkansas Fridge Magnet",
                            inStock = true,
                            count = 41,
                            user = seedUser,
                            userId = seedUser.id

                        };
                        Stock newStock4 = new Stock
                        {
                            itemName = "'Get Lost' Baseball Cap",
                            inStock = true,
                            count = 12,
                            user = seedUser,
                            userId = seedUser.id

                        };
                        Stock newStock5 = new Stock
                        {
                            itemName = "Devil's Den Postcard",
                            inStock = true,
                            count = 35,
                            user = seedUser,
                            userId = seedUser.id

                        };
                        Stock newStock6 = new Stock
                        {
                            itemName = "Black Bear Figurine",
                            inStock = true,
                            count = 8,
                            user = seedUser,
                            userId = seedUser.id

                        };
                        
                        context.Stock.AddRange(
                            newStock1,
                            newStock2,
                            newStock3,
                            newStock4,
                            newStock5,
                            newStock6
                        );

                        context.StockAlert.AddRange(
                            new StockAlert
                            {
                                active = true,
                                alertType = StockAlert.AlertType.lowStock,
                                lowStockThreshold = 10,
                                stock = newStock1,
                                stockId = newStock1.id,
                                itemName = newStock1.itemName,
                                user = seedUser,
                                userId = seedUser.id,

                            },
                            new StockAlert
                            {
                                active = false,
                                alertType = StockAlert.AlertType.outOfStock,
                                lowStockThreshold = 0,
                                stock = newStock2,
                                itemName = newStock2.itemName,
                                user = seedUser,
                                userId = seedUser.id,

                            },
                            new StockAlert
                            {
                                active = true,
                                alertType = StockAlert.AlertType.outOfStock,
                                lowStockThreshold = 10,
                                stock = newStock3,
                                stockId = newStock3.id,
                                itemName = newStock3.itemName,
                                user = seedUser,
                                userId = seedUser.id,

                            }
                        );

                        context.Order.AddRange(
                            new Order
                            {
                                stock = newStock1,
                                stockId = newStock1.id,
                                itemName = newStock1.itemName,
                                submitDate = DateTime.Now,
                                itemAmount = 50,
                                status = Order.Status.unsent,
                                user = seedUser,
                                userId = seedUser.id,

                            },
                            new Order
                            {
                                stock = newStock2,
                                stockId = newStock2.id,
                                itemName = newStock2.itemName,
                                submitDate = DateTime.Now,
                                itemAmount = 30,
                                status = Order.Status.sent,
                                user = seedUser,
                                userId = seedUser.id,

                            },
                            new Order
                            {
                                stock = newStock3,
                                stockId = newStock3.id,
                                itemName = newStock3.itemName,
                                submitDate = DateTime.Now,
                                itemAmount = 100,
                                status = Order.Status.shipped,
                                user = seedUser,
                                userId = seedUser.id,

                            }
                        );
                        
                        context.SaveChanges();
                    }
        }
    }
}
