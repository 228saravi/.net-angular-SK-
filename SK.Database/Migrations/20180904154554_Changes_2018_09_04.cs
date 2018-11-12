using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Database.Migrations
{
    public partial class Changes_2018_09_04 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Direction",
                table: "ChatMessages",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastSeenTime",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Direction",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "LastSeenTime",
                table: "AspNetUsers");
        }
    }
}
