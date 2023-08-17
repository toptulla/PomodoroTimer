using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PomodoroTimer.Api.Migrations
{
    public partial class StatItemUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "StatItems",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StatItems_UserId",
                table: "StatItems",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StatItems_AspNetUsers_UserId",
                table: "StatItems",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatItems_AspNetUsers_UserId",
                table: "StatItems");

            migrationBuilder.DropIndex(
                name: "IX_StatItems_UserId",
                table: "StatItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StatItems");
        }
    }
}
