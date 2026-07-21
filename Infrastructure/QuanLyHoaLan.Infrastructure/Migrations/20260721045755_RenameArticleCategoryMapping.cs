using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoaLan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameArticleCategoryMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleArticleCategories_ArticleCategories_CategoriesId",
                table: "ArticleArticleCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleArticleCategories_Articles_ArticlesId",
                table: "ArticleArticleCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArticleArticleCategories",
                table: "ArticleArticleCategories");

            migrationBuilder.RenameTable(
                name: "ArticleArticleCategories",
                newName: "ArticleCategoryMappings");

            migrationBuilder.RenameColumn(
                name: "ArticlesId",
                table: "ArticleCategoryMappings",
                newName: "ArticleId");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "ArticleCategoryMappings",
                newName: "ArticleCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ArticleArticleCategories_CategoriesId",
                table: "ArticleCategoryMappings",
                newName: "IX_ArticleCategoryMappings_ArticleCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArticleCategoryMappings",
                table: "ArticleCategoryMappings",
                columns: new[] { "ArticleId", "ArticleCategoryId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleCategoryMappings_ArticleCategories_ArticleCategoryId",
                table: "ArticleCategoryMappings",
                column: "ArticleCategoryId",
                principalTable: "ArticleCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleCategoryMappings_Articles_ArticleId",
                table: "ArticleCategoryMappings",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ArticleCategoryMappings_ArticleCategories_ArticleCategoryId",
                table: "ArticleCategoryMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_ArticleCategoryMappings_Articles_ArticleId",
                table: "ArticleCategoryMappings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ArticleCategoryMappings",
                table: "ArticleCategoryMappings");

            migrationBuilder.RenameTable(
                name: "ArticleCategoryMappings",
                newName: "ArticleArticleCategories");

            migrationBuilder.RenameColumn(
                name: "ArticleId",
                table: "ArticleArticleCategories",
                newName: "ArticlesId");

            migrationBuilder.RenameColumn(
                name: "ArticleCategoryId",
                table: "ArticleArticleCategories",
                newName: "CategoriesId");

            migrationBuilder.RenameIndex(
                name: "IX_ArticleCategoryMappings_ArticleCategoryId",
                table: "ArticleArticleCategories",
                newName: "IX_ArticleArticleCategories_CategoriesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ArticleArticleCategories",
                table: "ArticleArticleCategories",
                columns: new[] { "ArticlesId", "CategoriesId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleArticleCategories_ArticleCategories_CategoriesId",
                table: "ArticleArticleCategories",
                column: "CategoriesId",
                principalTable: "ArticleCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ArticleArticleCategories_Articles_ArticlesId",
                table: "ArticleArticleCategories",
                column: "ArticlesId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
