using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Busticket.Migrations
{
    /// <inheritdoc />
    public partial class roles1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "543f9a78-123b-4330-80e4-4672y8794549", null, "Empresa", "EMPRESA" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "543f9a78-123b-4330-80e4-4672y8794549");
        }
    }
}
