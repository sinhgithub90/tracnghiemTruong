using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestLinks.Data.Migrations
{
    public partial class DepartmentCourseUploadFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Department", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Department_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UploadLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeOpen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeClose = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CourseId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CourseYear = table.Column<int>(type: "int", nullable: false),
                    CourseTerm = table.Column<int>(type: "int", nullable: false),
                    MaxFileSize = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MaxFiles = table.Column<int>(type: "int", nullable: false),
                    AlllowFileTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HashedPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadLinks_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UploadFiles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Ext = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: true),
                    StudentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadLinkId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UploadFiles_UploadLinks_UploadLinkId",
                        column: x => x.UploadLinkId,
                        principalTable: "UploadLinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DepartmentId",
                table: "Courses",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadFiles_UploadLinkId",
                table: "UploadFiles",
                column: "UploadLinkId");

            migrationBuilder.CreateIndex(
                name: "IX_UploadLinks_CourseId",
                table: "UploadLinks",
                column: "CourseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UploadFiles");

            migrationBuilder.DropTable(
                name: "UploadLinks");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Department");
        }
    }
}
