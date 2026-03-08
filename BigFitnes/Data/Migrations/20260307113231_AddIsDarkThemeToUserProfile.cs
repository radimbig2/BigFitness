using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigFitnes.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDarkThemeToUserProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDarkTheme",
                table: "UserProfiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDarkTheme",
                table: "UserProfiles");
        }
    }
}
