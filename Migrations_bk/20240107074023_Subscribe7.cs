using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarikakeWeb.Migrations
{
    /// <inheritdoc />
    public partial class Subscribe7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CostDate",
                table: "TCostSubscribe");

            migrationBuilder.AddColumn<bool>(
                name: "r1",
                table: "TDateSubscribe",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "r2",
                table: "TDateSubscribe",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "r3",
                table: "TDateSubscribe",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "r4",
                table: "TDateSubscribe",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "r5",
                table: "TDateSubscribe",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "r1",
                table: "TDateSubscribe");

            migrationBuilder.DropColumn(
                name: "r2",
                table: "TDateSubscribe");

            migrationBuilder.DropColumn(
                name: "r3",
                table: "TDateSubscribe");

            migrationBuilder.DropColumn(
                name: "r4",
                table: "TDateSubscribe");

            migrationBuilder.DropColumn(
                name: "r5",
                table: "TDateSubscribe");

            migrationBuilder.AddColumn<DateTime>(
                name: "CostDate",
                table: "TCostSubscribe",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
