using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EfCore.Persistence.Migrations
{
    public partial class Add_Code_To_Entity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Product",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Customers");
        }
    }
}
