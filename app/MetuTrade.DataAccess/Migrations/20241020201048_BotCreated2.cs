using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MetuTrade.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class BotCreated2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModelUrl",
                table: "Bots",
                newName: "Url");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Url",
                table: "Bots",
                newName: "ModelUrl");
        }
    }
}
