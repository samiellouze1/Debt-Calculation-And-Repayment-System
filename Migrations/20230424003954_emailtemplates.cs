using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Debt_Calculation_And_Repayment_System.Migrations
{
    public partial class emailtemplates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EMAILTEMPLATEs",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NeedToLogin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMAILTEMPLATEs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EMAILTEMPLATEs");
        }
    }
}
