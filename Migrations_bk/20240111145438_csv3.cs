using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarikakeWeb.Migrations
{
    /// <inheritdoc />
    public partial class csv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "CsvMigration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    inputDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    buyDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    kindName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    buyAmount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pf1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pf2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pf3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pa1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pa2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pa3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pr1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pr2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pr3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    buyStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatePg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatePg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CsvMigration", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CsvMigration");
        }
    }
}
