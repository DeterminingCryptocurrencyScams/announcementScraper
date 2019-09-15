using Microsoft.EntityFrameworkCore.Migrations;

namespace AnnoucementScraper.Migrations
{
    public partial class Changedidtoupper : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Posts",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Posts",
                newName: "id");
        }
    }
}
