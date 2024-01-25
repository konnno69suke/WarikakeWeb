using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarikakeWeb.Migrations
{
    /// <inheritdoc />
    public partial class seq2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.RenameSequence(
            //    name: "CostIdSeq",
            //    schema: "WarikakeWebContext",
            //    newName: "CostIdSeq");

            migrationBuilder.AlterColumn<int>(
                name: "CostId",
                table: "TCost",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR CostIdSeq");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
         //   migrationBuilder.EnsureSchema(
         //       name: "WarikakeWebContext");

         //   migrationBuilder.RenameSequence(
         //       name: "CostIdSeq",
         //       newName: "CostIdSeq",
         //       newSchema: "WarikakeWebContext");

            migrationBuilder.AlterColumn<int>(
                name: "CostId",
                table: "TCost",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR CostIdSeq",
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
