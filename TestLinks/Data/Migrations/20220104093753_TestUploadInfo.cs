using Microsoft.EntityFrameworkCore.Migrations;

namespace TestLinks.Data.Migrations
{
    public partial class TestUploadInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UploadInfoId",
                table: "UploadFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UploadInfoLinkId",
                table: "UploadFiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadInfoStudentId",
                table: "UploadFiles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TestUploadInfo",
                columns: table => new
                {
                    StudentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LinkId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestUploadInfo", x => new { x.LinkId, x.StudentId });
                    table.ForeignKey(
                        name: "FK_TestUploadInfo_UploadLinks_LinkId",
                        column: x => x.LinkId,
                        principalTable: "UploadLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UploadFiles_UploadInfoLinkId_UploadInfoStudentId",
                table: "UploadFiles",
                columns: new[] { "UploadInfoLinkId", "UploadInfoStudentId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UploadFiles_TestUploadInfo_UploadInfoLinkId_UploadInfoStudentId",
                table: "UploadFiles",
                columns: new[] { "UploadInfoLinkId", "UploadInfoStudentId" },
                principalTable: "TestUploadInfo",
                principalColumns: new[] { "LinkId", "StudentId" },
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadFiles_TestUploadInfo_UploadInfoLinkId_UploadInfoStudentId",
                table: "UploadFiles");

            migrationBuilder.DropTable(
                name: "TestUploadInfo");

            migrationBuilder.DropIndex(
                name: "IX_UploadFiles_UploadInfoLinkId_UploadInfoStudentId",
                table: "UploadFiles");

            migrationBuilder.DropColumn(
                name: "UploadInfoId",
                table: "UploadFiles");

            migrationBuilder.DropColumn(
                name: "UploadInfoLinkId",
                table: "UploadFiles");

            migrationBuilder.DropColumn(
                name: "UploadInfoStudentId",
                table: "UploadFiles");
        }
    }
}
