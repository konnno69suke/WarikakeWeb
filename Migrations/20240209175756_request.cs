using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarikakeWeb.Migrations
{
    /// <inheritdoc />
    public partial class request : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReqType = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    ReqUserId = table.Column<int>(type: "int", nullable: false),
                    AgreeUserId = table.Column<int>(type: "int", nullable: false),
                    TempKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LimitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatePg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatePg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRequest", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TRequest");
        }
    }
}
