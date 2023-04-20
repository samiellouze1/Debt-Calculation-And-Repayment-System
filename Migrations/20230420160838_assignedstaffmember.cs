using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Debt_Calculation_And_Repayment_System.Migrations
{
    public partial class assignedstaffmember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "StaffMemberAssigned",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StaffMemberAssigned",
                table: "AspNetUsers");
        }
    }
}
