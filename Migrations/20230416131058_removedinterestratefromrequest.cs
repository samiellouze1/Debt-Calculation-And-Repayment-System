using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Debt_Calculation_And_Repayment_System.Migrations
{
    public partial class removedinterestratefromrequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterestRate",
                table: "REQUESTs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InterestRate",
                table: "REQUESTs",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
