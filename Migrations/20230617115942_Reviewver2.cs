using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicAlbumStore.Migrations
{
    public partial class Reviewver2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Review",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Review_UserId",
                table: "Review",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_AspNetUsers_UserId",
                table: "Review",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_AspNetUsers_UserId",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_UserId",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Review");
        }
    }
}
