using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoaLan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleCategoryType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ArticleCategories_Slug",
                table: "ArticleCategories");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ArticleCategories",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCategories_Type_Slug",
                table: "ArticleCategories",
                columns: new[] { "Type", "Slug" },
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ArticleCategories_Type_Slug",
                table: "ArticleCategories");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ArticleCategories");

            migrationBuilder.CreateIndex(
                name: "IX_ArticleCategories_Slug",
                table: "ArticleCategories",
                column: "Slug",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }
    }
}
