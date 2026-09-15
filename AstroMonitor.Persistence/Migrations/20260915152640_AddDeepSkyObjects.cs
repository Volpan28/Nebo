using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AstroMonitor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDeepSkyObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Stars",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Stars",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "SolarSystemBody",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Constellation",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DeepSkyObjects",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CatalogName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    RightAscension = table.Column<double>(type: "double precision", nullable: false),
                    Declination = table.Column<double>(type: "double precision", nullable: false),
                    Magnitude = table.Column<double>(type: "double precision", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ConstellationId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeepSkyObjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeepSkyObjects_Constellation_ConstellationId",
                        column: x => x.ConstellationId,
                        principalTable: "Constellation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeepSkyObjects_ConstellationId",
                table: "DeepSkyObjects",
                column: "ConstellationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeepSkyObjects");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Stars");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Stars");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "SolarSystemBody");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Constellation");
        }
    }
}
