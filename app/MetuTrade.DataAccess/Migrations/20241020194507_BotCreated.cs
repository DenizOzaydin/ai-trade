using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetuTrade.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class BotCreated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bar");

            migrationBuilder.CreateTable(
                name: "Bars",
                columns: table => new
                {
                    OpenTime = table.Column<long>(type: "bigint", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Interval = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Open = table.Column<double>(type: "float", nullable: false),
                    High = table.Column<double>(type: "float", nullable: false),
                    Low = table.Column<double>(type: "float", nullable: false),
                    Close = table.Column<double>(type: "float", nullable: false),
                    Volume = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bars", x => new { x.Symbol, x.Interval, x.OpenTime });
                });

            migrationBuilder.CreateTable(
                name: "Bots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModelUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bots", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bars");

            migrationBuilder.DropTable(
                name: "Bots");

            migrationBuilder.CreateTable(
                name: "Bar",
                columns: table => new
                {
                    Symbol = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Interval = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OpenTime = table.Column<long>(type: "bigint", nullable: false),
                    Close = table.Column<double>(type: "float", nullable: false),
                    High = table.Column<double>(type: "float", nullable: false),
                    Low = table.Column<double>(type: "float", nullable: false),
                    Open = table.Column<double>(type: "float", nullable: false),
                    Volume = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bar", x => new { x.Symbol, x.Interval, x.OpenTime });
                });
        }
    }
}
