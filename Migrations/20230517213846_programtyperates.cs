using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Debt_Calculation_And_Repayment_System.Migrations
{
    public partial class programtyperates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "PROGRAMTYPESs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "InterestRate",
                table: "PROGRAMTYPESs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestRateDelay",
                table: "PROGRAMTYPESs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestRateInstallment",
                table: "PROGRAMTYPESs",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "PROGRAMTYPESs");

            migrationBuilder.DropColumn(
                name: "InterestRate",
                table: "PROGRAMTYPESs");

            migrationBuilder.DropColumn(
                name: "InterestRateDelay",
                table: "PROGRAMTYPESs");

            migrationBuilder.DropColumn(
                name: "InterestRateInstallment",
                table: "PROGRAMTYPESs");
        }
    }
}
