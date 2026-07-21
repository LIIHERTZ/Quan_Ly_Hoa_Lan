using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoaLan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrchidSearchAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "BloomSeasons",
                table: "Orchids",
                type: "text[]",
                nullable: false,
                defaultValueSql: "ARRAY[]::text[]");

            migrationBuilder.AddColumn<List<string>>(
                name: "Colors",
                table: "Orchids",
                type: "text[]",
                nullable: false,
                defaultValueSql: "ARRAY[]::text[]");

            migrationBuilder.AddColumn<List<string>>(
                name: "Regions",
                table: "Orchids",
                type: "text[]",
                nullable: false,
                defaultValueSql: "ARRAY[]::text[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BloomSeasons",
                table: "Orchids");

            migrationBuilder.DropColumn(
                name: "Colors",
                table: "Orchids");

            migrationBuilder.DropColumn(
                name: "Regions",
                table: "Orchids");
        }
    }
}
