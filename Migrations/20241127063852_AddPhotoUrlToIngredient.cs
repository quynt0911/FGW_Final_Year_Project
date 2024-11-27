using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Blank.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoUrlToIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Photo",
                table: "Ingredients",
                newName: "PhotoUrl");

            migrationBuilder.AlterColumn<string>(
                name: "IngName",
                table: "Ingredients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "Ingredients",
                newName: "Photo");

            migrationBuilder.AlterColumn<string>(
                name: "IngName",
                table: "Ingredients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
