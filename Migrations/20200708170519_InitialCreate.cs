using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Inventory.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stock",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    itemName = table.Column<string>(nullable: true),
                    inStock = table.Column<bool>(nullable: false),
                    count = table.Column<int>(nullable: false),
                    userId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stock", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    userName = table.Column<string>(nullable: true),
                    pwHash = table.Column<byte[]>(nullable: true),
                    sessionID = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Vendor",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    phone = table.Column<string>(nullable: true),
                    address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "StockAlert",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    active = table.Column<bool>(nullable: false),
                    triggered = table.Column<bool>(nullable: false),
                    alertType = table.Column<int>(nullable: false),
                    itemName = table.Column<string>(nullable: true),
                    stockId = table.Column<int>(nullable: false),
                    lowStockThreshold = table.Column<int>(nullable: true),
                    userId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockAlert", x => x.id);
                    table.ForeignKey(
                        name: "FK_StockAlert_Stock_stockId",
                        column: x => x.stockId,
                        principalTable: "Stock",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(nullable: true),
                    Vendorid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.id);
                    table.ForeignKey(
                        name: "FK_Item_Vendor_Vendorid",
                        column: x => x.Vendorid,
                        principalTable: "Vendor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    stockId = table.Column<int>(nullable: false),
                    itemName = table.Column<string>(nullable: true),
                    submitDate = table.Column<DateTime>(nullable: false),
                    itemAmount = table.Column<int>(nullable: false),
                    status = table.Column<int>(nullable: false),
                    userId = table.Column<int>(nullable: false),
                    Vendorid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.id);
                    table.ForeignKey(
                        name: "FK_Order_Vendor_Vendorid",
                        column: x => x.Vendorid,
                        principalTable: "Vendor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Stock_stockId",
                        column: x => x.stockId,
                        principalTable: "Stock",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Item_Vendorid",
                table: "Item",
                column: "Vendorid");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Vendorid",
                table: "Order",
                column: "Vendorid");

            migrationBuilder.CreateIndex(
                name: "IX_Order_stockId",
                table: "Order",
                column: "stockId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAlert_stockId",
                table: "StockAlert",
                column: "stockId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "StockAlert");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Vendor");

            migrationBuilder.DropTable(
                name: "Stock");
        }
    }
}
