using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AituConnectAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChatId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TelegramUserId",
                table: "AspNetUsers",
                newName: "ChatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ChatId",
                table: "AspNetUsers",
                newName: "TelegramUserId");
        }
    }
}
