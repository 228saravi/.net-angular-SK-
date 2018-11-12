using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Database.Migrations
{
    public partial class Changes_2018_09_03__02 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessagesd_Connections_ConnectionId",
                table: "ChatMessagesd");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMessagesd",
                table: "ChatMessagesd");

            migrationBuilder.RenameTable(
                name: "ChatMessagesd",
                newName: "ChatMessages");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessagesd_ConnectionId",
                table: "ChatMessages",
                newName: "IX_ChatMessages_ConnectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMessages",
                table: "ChatMessages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Connections_ConnectionId",
                table: "ChatMessages",
                column: "ConnectionId",
                principalTable: "Connections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Connections_ConnectionId",
                table: "ChatMessages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatMessages",
                table: "ChatMessages");

            migrationBuilder.RenameTable(
                name: "ChatMessages",
                newName: "ChatMessagesd");

            migrationBuilder.RenameIndex(
                name: "IX_ChatMessages_ConnectionId",
                table: "ChatMessagesd",
                newName: "IX_ChatMessagesd_ConnectionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatMessagesd",
                table: "ChatMessagesd",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessagesd_Connections_ConnectionId",
                table: "ChatMessagesd",
                column: "ConnectionId",
                principalTable: "Connections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
