using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LecteurIptv.Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntityToFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserFavoriteVods",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserFavoriteVods",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserFavoriteChannels",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserFavoriteChannels",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserFavoriteVods");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserFavoriteVods");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserFavoriteChannels");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserFavoriteChannels");
        }
    }
}
