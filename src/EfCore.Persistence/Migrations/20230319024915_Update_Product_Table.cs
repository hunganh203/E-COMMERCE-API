using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfCore.Persistence.Migrations
{
    public partial class Update_Product_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Product",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "RateAvg",
                table: "Product",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "RateAvg",
                table: "Product");
        }
    }
}
