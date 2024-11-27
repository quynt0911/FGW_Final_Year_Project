using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blank.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDishMenuRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DishId",
                table: "Menus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DishId",
                table: "Menus",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
