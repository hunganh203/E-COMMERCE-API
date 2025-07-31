using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfCore.Persistence.Migrations
{
    public partial class Favicon_Website_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Article_Menu_MenuId",
                table: "Article");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Product_ProductId",
                table: "Review");

            migrationBuilder.AddColumn<string>(
                name: "Favicon",
                table: "Website",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "Review",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "MenuId",
                table: "Article",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Article_Menu_MenuId",
                table: "Article",
                column: "MenuId",
                principalTable: "Menu",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Product_ProductId",
                table: "Review",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Article_Menu_MenuId",
                table: "Article");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Product_ProductId",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "Favicon",
                table: "Website");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProductId",
                table: "Review",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MenuId",
                table: "Article",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Article_Menu_MenuId",
                table: "Article",
                column: "MenuId",
                principalTable: "Menu",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Product_ProductId",
                table: "Review",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
