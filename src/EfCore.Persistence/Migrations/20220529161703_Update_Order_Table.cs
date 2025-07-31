using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfCore.Persistence.Migrations
{
    public partial class Update_Order_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PriceShipping",
                table: "OrderDetail",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShippingStatus",
                table: "OrderDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "DiscountPrice",
                table: "Order",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancel",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinish",
                table: "Order",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SourceFrom",
                table: "Order",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "TotalPriceAfterDiscount",
                table: "Order",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceShipping",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "ShippingStatus",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "DiscountPrice",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IsCancel",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "IsFinish",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "SourceFrom",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "TotalPriceAfterDiscount",
                table: "Order");
        }
    }
}
