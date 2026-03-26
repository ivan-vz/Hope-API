using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hope.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Update_Menu_And_Meal_Models : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Menus_Name",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Meals_Name",
                table: "Meals");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_Name",
                table: "Menus",
                column: "Name",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_Name",
                table: "Meals",
                column: "Name",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Menus_Name",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Meals_Name",
                table: "Meals");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_Name",
                table: "Menus",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Meals_Name",
                table: "Meals",
                column: "Name",
                unique: true);
        }
    }
}
