using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloWorld.Migrations
{
    /// <inheritdoc />
    public partial class AddVendingMachineProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_VendingMachines_VendingMachineId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_VendingMachineId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VendingMachineId",
                table: "Products");

            migrationBuilder.CreateTable(
                name: "VendingMachineProducts",
                columns: table => new
                {
                    VendingMachineId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    id = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendingMachineProducts", x => new { x.VendingMachineId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_VendingMachineProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendingMachineProducts_VendingMachines_VendingMachineId",
                        column: x => x.VendingMachineId,
                        principalTable: "VendingMachines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_VendingMachineProducts_ProductId",
                table: "VendingMachineProducts",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendingMachineProducts");

            migrationBuilder.AddColumn<int>(
                name: "VendingMachineId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_VendingMachineId",
                table: "Products",
                column: "VendingMachineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_VendingMachines_VendingMachineId",
                table: "Products",
                column: "VendingMachineId",
                principalTable: "VendingMachines",
                principalColumn: "Id");
        }
    }
}
