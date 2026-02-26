using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Busticket.Migrations
{
    /// <inheritdoc />
    public partial class AddFechasRuta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaLlegada",
                table: "Ruta",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaSalida",
                table: "Ruta",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaLlegada",
                table: "Ruta");

            migrationBuilder.DropColumn(
                name: "FechaSalida",
                table: "Ruta");
        }
    }
}
