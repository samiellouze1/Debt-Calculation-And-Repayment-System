using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Debt_Calculation_And_Repayment_System.Migrations
{
    public partial class principalamountinterestamount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FromInterestAmount",
                table: "PAYMENTs",
                newName: "PrincipalAmount");

            migrationBuilder.RenameColumn(
                name: "FromInstallment",
                table: "PAYMENTs",
                newName: "InterestAmount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PrincipalAmount",
                table: "PAYMENTs",
                newName: "FromInterestAmount");

            migrationBuilder.RenameColumn(
                name: "InterestAmount",
                table: "PAYMENTs",
                newName: "FromInstallment");
        }
    }
}
