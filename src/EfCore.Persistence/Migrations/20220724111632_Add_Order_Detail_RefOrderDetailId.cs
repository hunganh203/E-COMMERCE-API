using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfCore.Persistence.Migrations
{
    public partial class Add_Order_Detail_RefOrderDetailId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefProductId",
                table: "OrderDetail");

            migrationBuilder.AddColumn<int>(
                name: "RefOrderDetailId",
                table: "OrderDetail",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefOrderDetailId",
                table: "OrderDetail");

            migrationBuilder.AddColumn<Guid>(
                name: "RefProductId",
                table: "OrderDetail",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
