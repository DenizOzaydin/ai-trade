using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetuTrade.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bar",
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
                    table.PrimaryKey("PK_Bar", x => new { x.Symbol, x.Interval, x.OpenTime });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bar");
        }
    }
}
