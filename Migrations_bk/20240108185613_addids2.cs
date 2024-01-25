using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarikakeWeb.Migrations
{
    /// <inheritdoc />
    public partial class addids2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MDefaultUser");

            migrationBuilder.DropTable(
                name: "MGroupGenre");

            migrationBuilder.DropColumn(
                name: "SubscriptDayOfMonth",
                table: "MGenre");

            migrationBuilder.DropColumn(
                name: "SubscriptFlg",
                table: "MGenre");

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "MGenre",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "MUser",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "MGroup",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GenreId",
                table: "MGenre",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MUser");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "MGroup");

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "MGenre");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "MGenre");

            migrationBuilder.AddColumn<int>(
                name: "SubscriptFlg",
                table: "MGenre",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubscriptDayOfMonth",
                table: "MGenre",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MDefaultUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatePg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    UpdatePg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MDefaultUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MGroupGenre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatePg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UpdatePg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MGroupGenre", x => x.Id);
                });
        }
    }
}
