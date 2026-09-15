using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AstroMonitor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SolarSystemBody_Entity_Extension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ArgumentOfPeriapsis",
                table: "SolarSystemBody",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Eccentricity",
                table: "SolarSystemBody",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Epoch",
                table: "SolarSystemBody",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Inclination",
                table: "SolarSystemBody",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "LongitudeOfAscendingNode",
                table: "SolarSystemBody",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "MeanAnomaly",
                table: "SolarSystemBody",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SemiMajorAxis",
                table: "SolarSystemBody",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArgumentOfPeriapsis",
                table: "SolarSystemBody");

            migrationBuilder.DropColumn(
                name: "Eccentricity",
                table: "SolarSystemBody");

            migrationBuilder.DropColumn(
                name: "Epoch",
                table: "SolarSystemBody");

            migrationBuilder.DropColumn(
                name: "Inclination",
                table: "SolarSystemBody");

            migrationBuilder.DropColumn(
                name: "LongitudeOfAscendingNode",
                table: "SolarSystemBody");

            migrationBuilder.DropColumn(
                name: "MeanAnomaly",
                table: "SolarSystemBody");

            migrationBuilder.DropColumn(
                name: "SemiMajorAxis",
                table: "SolarSystemBody");
        }
    }
}
