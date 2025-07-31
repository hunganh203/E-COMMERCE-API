using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfCore.Persistence.Migrations
{
    public partial class Updated_Review_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Created",
                table: "Review",
                newName: "UpdatedDate");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "Review",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "Review",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Review");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Review",
                newName: "Created");
        }
    }
}
