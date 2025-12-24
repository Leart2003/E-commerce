using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Punim_Diplome.Migrations
{
    /// <inheritdoc />
    public partial class MigrimiNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produktet_OrderProducts_OrderProductId",
                table: "Produktet");

            migrationBuilder.DropIndex(
                name: "IX_Produktet_OrderProductId",
                table: "Produktet");

            migrationBuilder.DropColumn(
                name: "OrderProductId",
                table: "Produktet");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryAddress",
                table: "OrderProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrderStatus",
                table: "OrderProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "OrderProducts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryAddress",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "OrderProducts");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "OrderProducts");

            migrationBuilder.AddColumn<int>(
                name: "OrderProductId",
                table: "Produktet",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Produktet_OrderProductId",
                table: "Produktet",
                column: "OrderProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produktet_OrderProducts_OrderProductId",
                table: "Produktet",
                column: "OrderProductId",
                principalTable: "OrderProducts",
                principalColumn: "Id");
        }
    }
}
