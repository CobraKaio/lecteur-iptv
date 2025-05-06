using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LecteurIptv.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddIsFeaturedToChannel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Channels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Channels");
        }
    }
}
