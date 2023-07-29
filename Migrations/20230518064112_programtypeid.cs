using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Debt_Calculation_And_Repayment_System.Migrations
{
    public partial class programtypeid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProgramTypeId",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ProgramTypeId",
                table: "AspNetUsers",
                column: "ProgramTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_PROGRAMTYPESs_ProgramTypeId",
                table: "AspNetUsers",
                column: "ProgramTypeId",
                principalTable: "PROGRAMTYPESs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_PROGRAMTYPESs_ProgramTypeId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ProgramTypeId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProgramTypeId",
                table: "AspNetUsers");
        }
    }
}
