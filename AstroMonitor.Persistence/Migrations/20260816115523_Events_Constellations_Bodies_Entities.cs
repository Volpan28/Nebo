using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AstroMonitor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Events_Constellations_Bodies_Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AstronomicalEvent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PeakDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsVisibleNakedEye = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AstronomicalEvent", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Constellation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    LatinName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EnglishName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Family = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Constellation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolarSystemBody",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BodyType = table.Column<string>(type: "text", nullable: false),
                    RadiusKm = table.Column<double>(type: "double precision", precision: 10, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TextureIdentifier = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolarSystemBody", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stars",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ProperName = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    RightAscension = table.Column<double>(type: "double precision", precision: 4, scale: 2, nullable: false),
                    Declination = table.Column<double>(type: "double precision", precision: 4, scale: 2, nullable: false),
                    Distance = table.Column<double>(type: "double precision", precision: 10, scale: 2, nullable: false),
                    Magnitude = table.Column<double>(type: "double precision", precision: 4, scale: 2, nullable: false),
                    ColorIndex = table.Column<double>(type: "double precision", precision: 4, scale: 2, nullable: false),
                    ConstellationId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stars_Constellation_ConstellationId",
                        column: x => x.ConstellationId,
                        principalTable: "Constellation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AstronomicalEvent_StartDate",
                table: "AstronomicalEvent",
                column: "StartDate");

            migrationBuilder.CreateIndex(
                name: "IX_Stars_ConstellationId",
                table: "Stars",
                column: "ConstellationId");

            migrationBuilder.CreateIndex(
                name: "IX_Stars_Magnitude",
                table: "Stars",
                column: "Magnitude");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AstronomicalEvent");

            migrationBuilder.DropTable(
                name: "SolarSystemBody");

            migrationBuilder.DropTable(
                name: "Stars");

            migrationBuilder.DropTable(
                name: "Constellation");
        }
    }
}
