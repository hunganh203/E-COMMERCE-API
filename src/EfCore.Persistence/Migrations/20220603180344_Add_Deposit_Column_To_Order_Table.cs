using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfCore.Persistence.Migrations
{
    public partial class Add_Deposit_Column_To_Order_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Deposit",
                table: "Order",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deposit",
                table: "Order");
        }
    }
}
