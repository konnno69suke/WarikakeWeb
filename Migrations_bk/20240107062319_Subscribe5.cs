using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarikakeWeb.Migrations
{
    /// <inheritdoc />
    public partial class Subscribe5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubscriptId",
                table: "TSubscribe",
                newName: "SubscribeId");

            migrationBuilder.RenameColumn(
                name: "SubscriptId",
                table: "TCostSubscribe",
                newName: "SubscribeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubscribeId",
                table: "TSubscribe",
                newName: "SubscriptId");

            migrationBuilder.RenameColumn(
                name: "SubscribeId",
                table: "TCostSubscribe",
                newName: "SubscriptId");
        }
    }
}
