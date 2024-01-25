using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarikakeWeb.Migrations
{
    /// <inheritdoc />
    public partial class Subscribe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TCostSubscribe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptId = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: true),
                    CostTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    GenreName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProvisionFlg = table.Column<int>(type: "int", nullable: false),
                    CostAmount = table.Column<int>(type: "int", nullable: false),
                    CostDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CostStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatePg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatePg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TCostSubscribe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TDateSubscribe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<int>(type: "int", nullable: true),
                    CostId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Boolean = table.Column<int>(type: "int", nullable: false),
                    m1 = table.Column<bool>(type: "bit", nullable: false),
                    m2 = table.Column<bool>(type: "bit", nullable: false),
                    m3 = table.Column<bool>(type: "bit", nullable: false),
                    m4 = table.Column<bool>(type: "bit", nullable: false),
                    m5 = table.Column<bool>(type: "bit", nullable: false),
                    m6 = table.Column<bool>(type: "bit", nullable: false),
                    m7 = table.Column<bool>(type: "bit", nullable: false),
                    m8 = table.Column<bool>(type: "bit", nullable: false),
                    m9 = table.Column<bool>(type: "bit", nullable: false),
                    m10 = table.Column<bool>(type: "bit", nullable: false),
                    m11 = table.Column<bool>(type: "bit", nullable: false),
                    m12 = table.Column<bool>(type: "bit", nullable: false),
                    w1 = table.Column<bool>(type: "bit", nullable: false),
                    w2 = table.Column<bool>(type: "bit", nullable: false),
                    w3 = table.Column<bool>(type: "bit", nullable: false),
                    w4 = table.Column<bool>(type: "bit", nullable: false),
                    w5 = table.Column<bool>(type: "bit", nullable: false),
                    w6 = table.Column<bool>(type: "bit", nullable: false),
                    w7 = table.Column<bool>(type: "bit", nullable: false),
                    d1 = table.Column<bool>(type: "bit", nullable: false),
                    d2 = table.Column<bool>(type: "bit", nullable: false),
                    d3 = table.Column<bool>(type: "bit", nullable: false),
                    d4 = table.Column<bool>(type: "bit", nullable: false),
                    d5 = table.Column<bool>(type: "bit", nullable: false),
                    d6 = table.Column<bool>(type: "bit", nullable: false),
                    d7 = table.Column<bool>(type: "bit", nullable: false),
                    d8 = table.Column<bool>(type: "bit", nullable: false),
                    d9 = table.Column<bool>(type: "bit", nullable: false),
                    d10 = table.Column<bool>(type: "bit", nullable: false),
                    d11 = table.Column<bool>(type: "bit", nullable: false),
                    d12 = table.Column<bool>(type: "bit", nullable: false),
                    d13 = table.Column<bool>(type: "bit", nullable: false),
                    d14 = table.Column<bool>(type: "bit", nullable: false),
                    d15 = table.Column<bool>(type: "bit", nullable: false),
                    d16 = table.Column<bool>(type: "bit", nullable: false),
                    d17 = table.Column<bool>(type: "bit", nullable: false),
                    d18 = table.Column<bool>(type: "bit", nullable: false),
                    d19 = table.Column<bool>(type: "bit", nullable: false),
                    d20 = table.Column<bool>(type: "bit", nullable: false),
                    d21 = table.Column<bool>(type: "bit", nullable: false),
                    d22 = table.Column<bool>(type: "bit", nullable: false),
                    d23 = table.Column<bool>(type: "bit", nullable: false),
                    d24 = table.Column<bool>(type: "bit", nullable: false),
                    d25 = table.Column<bool>(type: "bit", nullable: false),
                    d26 = table.Column<bool>(type: "bit", nullable: false),
                    d27 = table.Column<bool>(type: "bit", nullable: false),
                    d28 = table.Column<bool>(type: "bit", nullable: false),
                    d29 = table.Column<bool>(type: "bit", nullable: false),
                    d30 = table.Column<bool>(type: "bit", nullable: false),
                    d31 = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatePg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatePg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TDateSubscribe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TPaySubscribe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<int>(type: "int", nullable: true),
                    CostId = table.Column<int>(type: "int", nullable: false),
                    PayId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PayAmount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatePg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatePg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TPaySubscribe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TRepaySubscribe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<int>(type: "int", nullable: true),
                    CostId = table.Column<int>(type: "int", nullable: false),
                    RepayId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RepayAmount = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatePg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatePg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRepaySubscribe", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TSubscribe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<int>(type: "int", nullable: true),
                    SubscriptId = table.Column<int>(type: "int", nullable: false),
                    CostId = table.Column<int>(type: "int", nullable: false),
                    SubscribedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatePg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatePg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TSubscribe", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TCostSubscribe");

            migrationBuilder.DropTable(
                name: "TDateSubscribe");

            migrationBuilder.DropTable(
                name: "TPaySubscribe");

            migrationBuilder.DropTable(
                name: "TRepaySubscribe");

            migrationBuilder.DropTable(
                name: "TSubscribe");
        }
    }
}
