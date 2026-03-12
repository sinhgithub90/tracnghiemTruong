using Microsoft.EntityFrameworkCore.Migrations;

namespace TestLinks.Data.Migrations
{
    public partial class UploadInfoCheck : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Checked",
                table: "TestUploadInfos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Checked",
                table: "TestUploadInfos");
        }
    }
}
