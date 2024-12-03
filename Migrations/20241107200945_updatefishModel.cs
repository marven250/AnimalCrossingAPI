using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnimalCrossingAPI.Migrations
{
    /// <inheritdoc />
    public partial class updatefishModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Fish",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Fish");
        }
    }
}
