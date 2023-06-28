using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicAlbumStore.Migrations
{
    public partial class AddingMissingColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MusicAlbumName",
                table: "MusicAlbum",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MusicAlbumName",
                table: "MusicAlbum");
        }
    }
}
