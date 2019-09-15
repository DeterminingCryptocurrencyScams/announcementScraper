using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AnnoucementScraper.Migrations
{
    public partial class AnnTaskModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AnnTasks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PostTitle = table.Column<string>(nullable: true),
                    PostUrl = table.Column<string>(nullable: true),
                    Author = table.Column<string>(nullable: true),
                    Views = table.Column<int>(nullable: false),
                    Replies = table.Column<int>(nullable: false),
                    RetrievedAt = table.Column<DateTime>(nullable: false),
                    TaskStartedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnnTasks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AnnTasks");
        }
    }
}
