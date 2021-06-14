using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BugTracker.Migrations
{
    public partial class _004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_SenderID",
                table: "Notification");

            migrationBuilder.RenameColumn(
                name: "SenderID",
                table: "Notification",
                newName: "SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_SenderID",
                table: "Notification",
                newName: "IX_Notification_SenderId");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Created",
                table: "TicketAttachment",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_SenderId",
                table: "Notification",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_SenderId",
                table: "Notification");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Notification",
                newName: "SenderID");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_SenderId",
                table: "Notification",
                newName: "IX_Notification_SenderID");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "TicketAttachment",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_SenderID",
                table: "Notification",
                column: "SenderID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
