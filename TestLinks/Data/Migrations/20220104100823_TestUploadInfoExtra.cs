using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestLinks.Data.Migrations
{
    public partial class TestUploadInfoExtra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Fullname",
                table: "TestUploadInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "TestUploadInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSubmit",
                table: "TestUploadInfos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fullname",
                table: "TestUploadInfos");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "TestUploadInfos");

            migrationBuilder.DropColumn(
                name: "LastSubmit",
                table: "TestUploadInfos");
        }
    }
}
