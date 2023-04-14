using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Debt_Calculation_And_Repayment_System.Migrations
{
    public partial class greatmodelfixedlinks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InitialAmount",
                table: "DEBTs");

            migrationBuilder.RenameColumn(
                name: "PaidFull",
                table: "REQUESTs",
                newName: "ToBePaidInstallment");

            migrationBuilder.RenameColumn(
                name: "InterestRate",
                table: "DEBTs",
                newName: "Amount");

            migrationBuilder.RenameColumn(
                name: "DebtsTotal",
                table: "DEBTREGISTERs",
                newName: "TotalInstallmentAfterRequest");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegDate",
                table: "REQUESTs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "ToBePaidFull",
                table: "REQUESTs",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegDate",
                table: "PAYMENTs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "NumberOfDays",
                table: "INSTALLMENTs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegDate",
                table: "INSTALLMENTs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "NotPaidCash",
                table: "DEBTREGISTERs",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegDate",
                table: "DEBTREGISTERs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "DEBTREGISTERs",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAfterInterest",
                table: "DEBTREGISTERs",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAfterRequest",
                table: "DEBTREGISTERs",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCash",
                table: "DEBTREGISTERs",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalInstallment",
                table: "DEBTREGISTERs",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegDate",
                table: "REQUESTs");

            migrationBuilder.DropColumn(
                name: "ToBePaidFull",
                table: "REQUESTs");

            migrationBuilder.DropColumn(
                name: "RegDate",
                table: "PAYMENTs");

            migrationBuilder.DropColumn(
                name: "NumberOfDays",
                table: "INSTALLMENTs");

            migrationBuilder.DropColumn(
                name: "RegDate",
                table: "INSTALLMENTs");

            migrationBuilder.DropColumn(
                name: "NotPaidCash",
                table: "DEBTREGISTERs");

            migrationBuilder.DropColumn(
                name: "RegDate",
                table: "DEBTREGISTERs");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "DEBTREGISTERs");

            migrationBuilder.DropColumn(
                name: "TotalAfterInterest",
                table: "DEBTREGISTERs");

            migrationBuilder.DropColumn(
                name: "TotalAfterRequest",
                table: "DEBTREGISTERs");

            migrationBuilder.DropColumn(
                name: "TotalCash",
                table: "DEBTREGISTERs");

            migrationBuilder.DropColumn(
                name: "TotalInstallment",
                table: "DEBTREGISTERs");

            migrationBuilder.RenameColumn(
                name: "ToBePaidInstallment",
                table: "REQUESTs",
                newName: "PaidFull");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "DEBTs",
                newName: "InterestRate");

            migrationBuilder.RenameColumn(
                name: "TotalInstallmentAfterRequest",
                table: "DEBTREGISTERs",
                newName: "DebtsTotal");

            migrationBuilder.AddColumn<decimal>(
                name: "InitialAmount",
                table: "DEBTs",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
