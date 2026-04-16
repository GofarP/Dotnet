using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelloWorld.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVendingMachineIdFromProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_VendingMachine_VendingMachineId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VendingMachine",
                table: "VendingMachine");

            migrationBuilder.RenameTable(
                name: "VendingMachine",
                newName: "VendingMachines");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VendingMachines",
                table: "VendingMachines",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_VendingMachines_VendingMachineId",
                table: "Products",
                column: "VendingMachineId",
                principalTable: "VendingMachines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_VendingMachines_VendingMachineId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VendingMachines",
                table: "VendingMachines");

            migrationBuilder.RenameTable(
                name: "VendingMachines",
                newName: "VendingMachine");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VendingMachine",
                table: "VendingMachine",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_VendingMachine_VendingMachineId",
                table: "Products",
                column: "VendingMachineId",
                principalTable: "VendingMachine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
