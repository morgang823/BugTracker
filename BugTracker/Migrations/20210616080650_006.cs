using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTracker.Migrations
{
    public partial class _006 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Ticket",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ticket_CompanyId",
                table: "Ticket",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ticket_Company_CompanyId",
                table: "Ticket",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ticket_Company_CompanyId",
                table: "Ticket");

            migrationBuilder.DropIndex(
                name: "IX_Ticket_CompanyId",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Ticket");
        }
    }
}
