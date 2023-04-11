using Microsoft.EntityFrameworkCore.Migrations;


#nullable disable

namespace Debt_Calculation_And_Repayment_System.Migrations
{
    public partial class @fixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountAfterInstallments",
                table: "PAYMENTPLANs",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountAfterInstallments",
                table: "PAYMENTPLANs");
        }
    }
}
