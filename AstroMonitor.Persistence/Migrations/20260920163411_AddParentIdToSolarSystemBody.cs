using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AstroMonitor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddParentIdToSolarSystemBody : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentId",
                table: "SolarSystemBody",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "SolarSystemBody");
        }
    }
}
