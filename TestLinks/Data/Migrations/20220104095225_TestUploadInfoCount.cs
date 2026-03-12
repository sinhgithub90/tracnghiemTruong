using Microsoft.EntityFrameworkCore.Migrations;

namespace TestLinks.Data.Migrations
{
    public partial class TestUploadInfoCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestUploadInfo_UploadLinks_LinkId",
                table: "TestUploadInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadFiles_TestUploadInfo_UploadInfoLinkId_UploadInfoStudentId",
                table: "UploadFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestUploadInfo",
                table: "TestUploadInfo");

            migrationBuilder.RenameTable(
                name: "TestUploadInfo",
                newName: "TestUploadInfos");

            migrationBuilder.AddColumn<int>(
                name: "FileCount",
                table: "TestUploadInfos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestUploadInfos",
                table: "TestUploadInfos",
                columns: new[] { "LinkId", "StudentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TestUploadInfos_UploadLinks_LinkId",
                table: "TestUploadInfos",
                column: "LinkId",
                principalTable: "UploadLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadFiles_TestUploadInfos_UploadInfoLinkId_UploadInfoStudentId",
                table: "UploadFiles",
                columns: new[] { "UploadInfoLinkId", "UploadInfoStudentId" },
                principalTable: "TestUploadInfos",
                principalColumns: new[] { "LinkId", "StudentId" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestUploadInfos_UploadLinks_LinkId",
                table: "TestUploadInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_UploadFiles_TestUploadInfos_UploadInfoLinkId_UploadInfoStudentId",
                table: "UploadFiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestUploadInfos",
                table: "TestUploadInfos");

            migrationBuilder.DropColumn(
                name: "FileCount",
                table: "TestUploadInfos");

            migrationBuilder.RenameTable(
                name: "TestUploadInfos",
                newName: "TestUploadInfo");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestUploadInfo",
                table: "TestUploadInfo",
                columns: new[] { "LinkId", "StudentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TestUploadInfo_UploadLinks_LinkId",
                table: "TestUploadInfo",
                column: "LinkId",
                principalTable: "UploadLinks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UploadFiles_TestUploadInfo_UploadInfoLinkId_UploadInfoStudentId",
                table: "UploadFiles",
                columns: new[] { "UploadInfoLinkId", "UploadInfoStudentId" },
                principalTable: "TestUploadInfo",
                principalColumns: new[] { "LinkId", "StudentId" },
                onDelete: ReferentialAction.Restrict);
        }
    }
}
