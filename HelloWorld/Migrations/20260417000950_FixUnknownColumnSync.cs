using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloWorld.Migrations
{
    /// <inheritdoc />
    public partial class FixUnknownColumnSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
