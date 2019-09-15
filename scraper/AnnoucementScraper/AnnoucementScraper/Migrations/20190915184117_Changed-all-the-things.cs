using Microsoft.EntityFrameworkCore.Migrations;

namespace AnnoucementScraper.Migrations
{
    public partial class Changedallthethings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Contents",
                table: "Posts",
                newName: "TopicUrl");

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TopicAuthor",
                table: "Posts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TopicTitle",
                table: "Posts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Body",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "TopicAuthor",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "TopicTitle",
                table: "Posts");

            migrationBuilder.RenameColumn(
                name: "TopicUrl",
                table: "Posts",
                newName: "Contents");
        }
    }
}
