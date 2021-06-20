using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTracker.Migrations
{
    public partial class _007 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketAttachmentId",
                table: "TicketHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TicketCommentId",
                table: "TicketHistory",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistory_TicketAttachmentId",
                table: "TicketHistory",
                column: "TicketAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TicketHistory_TicketCommentId",
                table: "TicketHistory",
                column: "TicketCommentId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketHistory_TicketAttachment_TicketAttachmentId",
                table: "TicketHistory",
                column: "TicketAttachmentId",
                principalTable: "TicketAttachment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketHistory_TicketComment_TicketCommentId",
                table: "TicketHistory",
                column: "TicketCommentId",
                principalTable: "TicketComment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketHistory_TicketAttachment_TicketAttachmentId",
                table: "TicketHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketHistory_TicketComment_TicketCommentId",
                table: "TicketHistory");

            migrationBuilder.DropIndex(
                name: "IX_TicketHistory_TicketAttachmentId",
                table: "TicketHistory");

            migrationBuilder.DropIndex(
                name: "IX_TicketHistory_TicketCommentId",
                table: "TicketHistory");

            migrationBuilder.DropColumn(
                name: "TicketAttachmentId",
                table: "TicketHistory");

            migrationBuilder.DropColumn(
                name: "TicketCommentId",
                table: "TicketHistory");
        }
    }
}
