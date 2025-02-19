using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Punim_Diplome.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProduktKomentRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_IdentityUserId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_IdentityUserId",
                table: "Comments",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_IdentityUserId",
                table: "Comments");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_IdentityUserId",
                table: "Comments",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
