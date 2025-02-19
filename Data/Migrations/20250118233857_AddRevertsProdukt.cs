using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Punim_Diplome.Migrations
{
    /// <inheritdoc />
    public partial class AddRevertsProdukt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produktet_AspNetUsers_IdentityUserId",
                table: "Produktet");

            migrationBuilder.DropIndex(
                name: "IX_Produktet_IdentityUserId",
                table: "Produktet");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Produktet");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Produktet",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Produktet_IdentityUserId",
                table: "Produktet",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produktet_AspNetUsers_IdentityUserId",
                table: "Produktet",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
