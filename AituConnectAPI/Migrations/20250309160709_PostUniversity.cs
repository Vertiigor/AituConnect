using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AituConnectAPI.Migrations
{
    /// <inheritdoc />
    public partial class PostUniversity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "University",
                table: "Posts",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "University",
                table: "Posts");
        }
    }
}
