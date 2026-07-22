using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IzinSistemi_Back.Migrations
{
    /// <inheritdoc />
    public partial class RenamePersonelToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_Personels_PersonelId",
                table: "Leaves");

            migrationBuilder.DropTable(
                name: "Personels");

            migrationBuilder.RenameColumn(
                name: "PersonelId",
                table: "Leaves",
                newName: "EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Leaves_PersonelId",
                table: "Leaves",
                newName: "IX_Leaves_EmployeeId");

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Surname = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Department = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    TotalLeaveDays = table.Column<int>(type: "INTEGER", nullable: true),
                    RemainingLeaveDays = table.Column<int>(type: "INTEGER", nullable: true),
                    LeaveReset = table.Column<DateTime>(type: "TEXT", nullable: true),
                    BirthDay = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_Employees_EmployeeId",
                table: "Leaves",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leaves_Employees_EmployeeId",
                table: "Leaves");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Leaves",
                newName: "PersonelId");

            migrationBuilder.RenameIndex(
                name: "IX_Leaves_EmployeeId",
                table: "Leaves",
                newName: "IX_Leaves_PersonelId");

            migrationBuilder.CreateTable(
                name: "Personels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BirthDay = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Department = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    LeaveReset = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    RemainingLeaveDays = table.Column<int>(type: "INTEGER", nullable: true),
                    Surname = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    TotalLeaveDays = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personels", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Leaves_Personels_PersonelId",
                table: "Leaves",
                column: "PersonelId",
                principalTable: "Personels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
