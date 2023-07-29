using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Debt_Calculation_And_Repayment_System.Migrations
{
    public partial class FirsInsDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FirstInstallmentDate",
                table: "DEBTREGISTERs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProgramFinishDate",
                table: "DEBTREGISTERs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstInstallmentDate",
                table: "DEBTREGISTERs");

            migrationBuilder.DropColumn(
                name: "ProgramFinishDate",
                table: "DEBTREGISTERs");
        }
    }
}
