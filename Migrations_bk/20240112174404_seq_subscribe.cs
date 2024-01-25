using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarikakeWeb.Migrations
{
    /// <inheritdoc />
    public partial class seq_subscribe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "SubscribeIdSeq",
                startValue: 12L);

            migrationBuilder.AlterColumn<int>(
                name: "SubscribeId",
                table: "TCostSubscribe",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR SubscribeIdSeq",
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropSequence(
                name: "SubscribeIdSeq");

            migrationBuilder.AlterColumn<int>(
                name: "SubscribeId",
                table: "TCostSubscribe",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR SubscribeIdSeq");
        }
    }
}
