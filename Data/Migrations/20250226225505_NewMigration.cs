using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Punim_Diplome.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
