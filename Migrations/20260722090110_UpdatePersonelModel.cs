using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IzinSistemi_Back.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePersonelModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NameSurname",
                table: "Personels",
                newName: "Surname");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LeaveReset",
                table: "Personels",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDay",
                table: "Personels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Personels",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthDay",
                table: "Personels");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Personels");

            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "Personels",
                newName: "NameSurname");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LeaveReset",
                table: "Personels",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "TEXT",
                oldNullable: true);
        }
    }
}
