using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarikakeWeb.Migrations
{
    /// <inheritdoc />
    public partial class delGenreFlg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreviousCostId",
                table: "MGenre");

            migrationBuilder.DropColumn(
                name: "PreviousFlg",
                table: "MGenre");

            migrationBuilder.AlterColumn<int>(
                name: "SubscriptDayOfMonth",
                table: "MGenre",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SubscriptDayOfMonth",
                table: "MGenre",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PreviousCostId",
                table: "MGenre",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PreviousFlg",
                table: "MGenre",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
