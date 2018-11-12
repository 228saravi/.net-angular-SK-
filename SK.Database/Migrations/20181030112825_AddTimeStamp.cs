using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SK.Database.Migrations
{
    public partial class AddTimeStamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Vacancies",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "ExpertProfiles",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Events",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Connections",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "AspNetUsers",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Vacancies");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "ExpertProfiles");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Connections");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "AspNetUsers");
        }
    }
}
