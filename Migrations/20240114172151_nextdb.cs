using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarikakeWeb.Migrations
{
    /// <inheritdoc />
    public partial class nextdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence<int>(
                name: "CostIdSeq",
                startValue: 107L);

            migrationBuilder.CreateSequence<int>(
                name: "SubscribeIdSeq",
                startValue: 12L);

            migrationBuilder.CreateTable(
                name: "CsvMigration",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<int>(type: "int", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "MGenre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<int>(type: "int", nullable: true),
                    GenreName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatePg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatePg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MGenre", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MGroup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<int>(type: "int", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatePg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatePg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<int>(type: "int", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatePg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatePg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MMember", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatePg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatePg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TCost",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CostId = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR CostIdSeq"),
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
                    table.PrimaryKey("PK_TCost", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TCostSubscribe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscribeId = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR SubscribeIdSeq"),
                    status = table.Column<int>(type: "int", nullable: true),
                    CostTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupId = table.Column<int>(type: "int", nullable: false),
                    GenreId = table.Column<int>(type: "int", nullable: false),
                    GenreName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProvisionFlg = table.Column<int>(type: "int", nullable: false),
                    CostAmount = table.Column<int>(type: "int", nullable: false),
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
                    SubscribeId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
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
                    r1 = table.Column<bool>(type: "bit", nullable: false),
                    r2 = table.Column<bool>(type: "bit", nullable: false),
                    r3 = table.Column<bool>(type: "bit", nullable: false),
                    r4 = table.Column<bool>(type: "bit", nullable: false),
                    r5 = table.Column<bool>(type: "bit", nullable: false),
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
                name: "TPay",
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
                    table.PrimaryKey("PK_TPay", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TPaySubscribe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<int>(type: "int", nullable: true),
                    SubscribeId = table.Column<int>(type: "int", nullable: false),
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
                name: "TRepay",
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
                    table.PrimaryKey("PK_TRepay", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TRepaySubscribe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    status = table.Column<int>(type: "int", nullable: true),
                    SubscribeId = table.Column<int>(type: "int", nullable: false),
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
                    SubscribeId = table.Column<int>(type: "int", nullable: false),
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
                name: "CsvMigration");

            migrationBuilder.DropTable(
                name: "MGenre");

            migrationBuilder.DropTable(
                name: "MGroup");

            migrationBuilder.DropTable(
                name: "MMember");

            migrationBuilder.DropTable(
                name: "MUser");

            migrationBuilder.DropTable(
                name: "TCost");

            migrationBuilder.DropTable(
                name: "TCostSubscribe");

            migrationBuilder.DropTable(
                name: "TDateSubscribe");

            migrationBuilder.DropTable(
                name: "TPay");

            migrationBuilder.DropTable(
                name: "TPaySubscribe");

            migrationBuilder.DropTable(
                name: "TRepay");

            migrationBuilder.DropTable(
                name: "TRepaySubscribe");

            migrationBuilder.DropTable(
                name: "TSubscribe");

            migrationBuilder.DropSequence(
                name: "CostIdSeq");

            migrationBuilder.DropSequence(
                name: "SubscribeIdSeq");
        }
    }
}
