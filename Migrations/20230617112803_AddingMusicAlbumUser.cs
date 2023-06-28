using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicAlbumStore.Migrations
{
    public partial class AddingMusicAlbumUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MusicAlbumUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MusicAlbumId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicAlbumUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MusicAlbumUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MusicAlbumUser_MusicAlbum_MusicAlbumId",
                        column: x => x.MusicAlbumId,
                        principalTable: "MusicAlbum",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MusicAlbumUser_MusicAlbumId",
                table: "MusicAlbumUser",
                column: "MusicAlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicAlbumUser_UserId",
                table: "MusicAlbumUser",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MusicAlbumUser");
        }
    }
}
