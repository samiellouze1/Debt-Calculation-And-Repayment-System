using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Debt_Calculation_And_Repayment_System.Migrations
{
    public partial class rate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProgramID",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestRate",
                table: "DEBTREGISTERs",
                type: "decimal(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestRateDelay",
                table: "DEBTREGISTERs",
                type: "decimal(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InterestRateInstallment",
                table: "DEBTREGISTERs",
                type: "decimal(10,4)",
                precision: 10,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterestRateDelay",
                table: "DEBTREGISTERs");

            migrationBuilder.DropColumn(
                name: "InterestRateInstallment",
                table: "DEBTREGISTERs");

            migrationBuilder.AlterColumn<decimal>(
                name: "InterestRate",
                table: "DEBTREGISTERs",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,4)",
                oldPrecision: 10,
                oldScale: 4);

            migrationBuilder.AddColumn<string>(
                name: "ProgramID",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
