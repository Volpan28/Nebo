using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AstroMonitor.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeStarFieldsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stars_Constellation_ConstellationId",
                table: "Stars");

            migrationBuilder.AlterColumn<string>(
                name: "ProperName",
                table: "Stars",
                type: "character varying(40)",
                maxLength: 40,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "ConstellationId",
                table: "Stars",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<double>(
                name: "ColorIndex",
                table: "Stars",
                type: "double precision",
                precision: 4,
                scale: 2,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 4,
                oldScale: 2);

            migrationBuilder.AddForeignKey(
                name: "FK_Stars_Constellation_ConstellationId",
                table: "Stars",
                column: "ConstellationId",
                principalTable: "Constellation",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stars_Constellation_ConstellationId",
                table: "Stars");

            migrationBuilder.AlterColumn<string>(
                name: "ProperName",
                table: "Stars",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ConstellationId",
                table: "Stars",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ColorIndex",
                table: "Stars",
                type: "double precision",
                precision: 4,
                scale: 2,
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldPrecision: 4,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Stars_Constellation_ConstellationId",
                table: "Stars",
                column: "ConstellationId",
                principalTable: "Constellation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
