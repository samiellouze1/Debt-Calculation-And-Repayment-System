using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Debt_Calculation_And_Repayment_System.Migrations
{
    public partial class greatmodelfixed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_DEBTREGISTERs_Id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_DEBTREGISTERs_REQUESTs_Id",
                table: "DEBTREGISTERs");

            migrationBuilder.AddForeignKey(
                name: "FK_DEBTREGISTERs_AspNetUsers_Id",
                table: "DEBTREGISTERs",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_REQUESTs_DEBTREGISTERs_Id",
                table: "REQUESTs",
                column: "Id",
                principalTable: "DEBTREGISTERs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DEBTREGISTERs_AspNetUsers_Id",
                table: "DEBTREGISTERs");

            migrationBuilder.DropForeignKey(
                name: "FK_REQUESTs_DEBTREGISTERs_Id",
                table: "REQUESTs");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_DEBTREGISTERs_Id",
                table: "AspNetUsers",
                column: "Id",
                principalTable: "DEBTREGISTERs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DEBTREGISTERs_REQUESTs_Id",
                table: "DEBTREGISTERs",
                column: "Id",
                principalTable: "REQUESTs",
                principalColumn: "Id");
        }
    }
}
