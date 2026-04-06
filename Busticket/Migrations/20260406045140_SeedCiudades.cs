using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Busticket.Migrations
{
    /// <inheritdoc />
    public partial class SeedCiudades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Ciudad",
                columns: new[] { "CiudadId", "Lat", "Lng", "Nombre" },
                values: new object[,]
                {
                    { 7, 4.8133299999999997, -75.696110000000004, "Pereira" },
                    { 8, 5.0702800000000003, -75.513890000000004, "Manizales" },
                    { 9, 11.240780000000001, -74.199039999999997, "Santa Marta" },
                    { 10, 4.4388899999999998, -75.232219999999998, "Ibagué" },
                    { 11, 2.92767, -75.281940000000006, "Neiva" },
                    { 12, 7.8891, -72.496899999999997, "Cúcuta" },
                    { 13, 9.3044399999999996, -75.397220000000004, "Sincelejo" },
                    { 14, 10.463329999999999, -73.253060000000005, "Valledupar" },
                    { 15, 2.4441000000000002, -76.614500000000007, "Popayán" },
                    { 16, 1.2136100000000001, -77.281109999999998, "Pasto" },
                    { 17, 5.5338900000000004, -73.367779999999996, "Tunja" },
                    { 18, 8.7477800000000006, -75.881110000000007, "Montería" },
                    { 19, 7.0694400000000002, -73.087779999999995, "Floridablanca" },
                    { 20, 11.54472, -72.907219999999995, "Riohacha" },
                    { 21, 5.3377800000000004, -72.391109999999998, "Yopal" },
                    { 22, 7.0880599999999996, -70.762219999999999, "Arauca" },
                    { 23, 10.83361, -74.833609999999993, "Malambo" },
                    { 24, 6.1744399999999997, -75.611940000000004, "Itagüí" },
                    { 25, 6.17028, -75.577500000000001, "Envigado" },
                    { 26, 10.91639, -74.766390000000001, "Soledad" },
                    { 27, 6.3375000000000004, -75.558329999999998, "Bello" },
                    { 28, 5.5361099999999999, -73.367500000000007, "Tunja" },
                    { 29, 3.8936099999999998, -77.067220000000006, "Buenaventura" },
                    { 30, 4.8944400000000003, -75.316109999999995, "Santa Rosa de Cabal" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Ciudad",
                keyColumn: "CiudadId",
                keyValue: 30);
        }
    }
}
