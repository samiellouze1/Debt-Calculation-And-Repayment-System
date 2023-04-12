using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Debt_Calculation_And_Repayment_System.Migrations
{
    public partial class @fixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "NumOfMonths",
                table: "REQUESTs",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "REQUESTs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "REQUESTs");

            migrationBuilder.AlterColumn<string>(
                name: "NumOfMonths",
                table: "REQUESTs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
