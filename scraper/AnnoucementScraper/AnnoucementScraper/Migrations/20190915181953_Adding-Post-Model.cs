using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnnoucementScraper.Migrations
{
    public partial class AddingPostModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    UserId = table.Column<int>(nullable: false),
                    Merit = table.Column<int>(nullable: false),
                    Activity = table.Column<int>(nullable: false),
                    Position = table.Column<int>(nullable: false),
                    Contents = table.Column<string>(nullable: true),
                    PostedAt = table.Column<string>(nullable: true),
                    TaskId = table.Column<int>(nullable: false),
                    PostNumber = table.Column<int>(nullable: false),
                    RetrievedAt = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
