using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicAlbumStore.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArtistName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Biography = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    Website = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Label = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FormationYear = table.Column<int>(type: "int", nullable: true),
                    DisbandmentYear = table.Column<int>(type: "int", nullable: true),
                    ArtistImage = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artist", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GenreName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genre", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MusicAlbum",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReleaseDate = table.Column<DateTime>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    Language = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    LengthInMinutes = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumSongs = table.Column<int>(type: "int", nullable: false),
                    CoverImage = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    PdfUrl = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: true),
                    ArtistId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicAlbum", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MusicAlbum_Artist_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artist",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MusicAlbumGenre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MusicAlbumId = table.Column<int>(type: "int", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicAlbumGenre", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MusicAlbumGenre_Genre_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genre",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MusicAlbumGenre_MusicAlbum_MusicAlbumId",
                        column: x => x.MusicAlbumId,
                        principalTable: "MusicAlbum",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Review",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    MusicAlbumId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Review", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Review_MusicAlbum_MusicAlbumId",
                        column: x => x.MusicAlbumId,
                        principalTable: "MusicAlbum",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MusicAlbum_ArtistId",
                table: "MusicAlbum",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicAlbumGenre_GenreId",
                table: "MusicAlbumGenre",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicAlbumGenre_MusicAlbumId",
                table: "MusicAlbumGenre",
                column: "MusicAlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_Review_MusicAlbumId",
                table: "Review",
                column: "MusicAlbumId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MusicAlbumGenre");

            migrationBuilder.DropTable(
                name: "Review");

            migrationBuilder.DropTable(
                name: "Genre");

            migrationBuilder.DropTable(
                name: "MusicAlbum");

            migrationBuilder.DropTable(
                name: "Artist");
        }
    }
}
