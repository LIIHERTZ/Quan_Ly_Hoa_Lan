using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoaLan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OrchidManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orchids_Categories_CategoryId",
                table: "Orchids");

            migrationBuilder.DropIndex(
                name: "IX_Orchids_CategoryId",
                table: "Orchids");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Orchids");

            migrationBuilder.CreateTable(
                name: "CategoryOrchid",
                columns: table => new
                {
                    CategoriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrchidsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryOrchid", x => new { x.CategoriesId, x.OrchidsId });
                    table.ForeignKey(
                        name: "FK_CategoryOrchid_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryOrchid_Orchids_OrchidsId",
                        column: x => x.OrchidsId,
                        principalTable: "Orchids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryOrchid_OrchidsId",
                table: "CategoryOrchid",
                column: "OrchidsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryOrchid");

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Orchids",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Orchids_CategoryId",
                table: "Orchids",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orchids_Categories_CategoryId",
                table: "Orchids",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
