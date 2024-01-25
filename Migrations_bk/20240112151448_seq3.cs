using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarikakeWeb.Migrations
{
    /// <inheritdoc />
    public partial class seq3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.AlterColumn<int>(
                name: "CostId",
                table: "TCost",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR CostIdSeq",
                oldClrType: typeof(int),
                oldType: "int");
            */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.AlterColumn<int>(
                name: "CostId",
                table: "TCost",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR CostIdSeq");
            */
        }
    }
}
