using Microsoft.EntityFrameworkCore.Migrations;

namespace TestLinks.Data.Migrations
{
    public partial class TestDuration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "UploadLinks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExtraTime",
                table: "UploadLinks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "UploadLinks");

            migrationBuilder.DropColumn(
                name: "ExtraTime",
                table: "UploadLinks");
        }
    }
}
