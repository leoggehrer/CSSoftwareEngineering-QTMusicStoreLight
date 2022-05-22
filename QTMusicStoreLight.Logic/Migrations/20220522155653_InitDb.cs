using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QTMusicStoreLight.Logic.Migrations
{
    public partial class InitDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "App");

            migrationBuilder.CreateTable(
                name: "Songs",
                schema: "App",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Album = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Artist = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Composer = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Millisconds = table.Column<long>(type: "bigint", nullable: false),
                    Bytes = table.Column<long>(type: "bigint", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Songs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Songs_Album_Artist_Genre_Title",
                schema: "App",
                table: "Songs",
                columns: new[] { "Album", "Artist", "Genre", "Title" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Songs",
                schema: "App");
        }
    }
}
