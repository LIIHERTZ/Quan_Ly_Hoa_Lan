using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoaLan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleTypeAndCategoryConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Articles",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "CULTIVATION");

            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT mapping."ArticleId"
                        FROM "ArticleCategoryMappings" AS mapping
                        INNER JOIN "ArticleCategories" AS category
                            ON category."Id" = mapping."ArticleCategoryId"
                        GROUP BY mapping."ArticleId"
                        HAVING COUNT(DISTINCT category."Type") > 1
                    ) THEN
                        RAISE EXCEPTION 'Có bài viết đang thuộc đồng thời nhiều nhóm danh mục.';
                    END IF;
                END;
                $$;

                UPDATE "Articles" AS article
                SET "Type" = category."Type"
                FROM "ArticleCategoryMappings" AS mapping
                INNER JOIN "ArticleCategories" AS category
                    ON category."Id" = mapping."ArticleCategoryId"
                WHERE mapping."ArticleId" = article."Id";
                """);

            migrationBuilder.Sql(
                """
                CREATE FUNCTION validate_article_category_mapping()
                RETURNS trigger
                LANGUAGE plpgsql
                AS $$
                DECLARE
                    article_type character varying(32);
                    category_type character varying(32);
                BEGIN
                    SELECT "Type" INTO article_type
                    FROM "Articles"
                    WHERE "Id" = NEW."ArticleId";

                    SELECT "Type" INTO category_type
                    FROM "ArticleCategories"
                    WHERE "Id" = NEW."ArticleCategoryId" AND NOT "IsDeleted";

                    IF article_type IS DISTINCT FROM category_type THEN
                        RAISE EXCEPTION 'Loại bài viết và loại danh mục không khớp.'
                            USING ERRCODE = '23514',
                                  CONSTRAINT = 'CK_ArticleCategoryMappings_MatchingType';
                    END IF;

                    IF EXISTS (
                        SELECT 1
                        FROM "ArticleCategories"
                        WHERE "ParentId" = NEW."ArticleCategoryId" AND NOT "IsDeleted"
                    ) THEN
                        RAISE EXCEPTION 'Chỉ được gắn bài viết vào danh mục lá.'
                            USING ERRCODE = '23514',
                                  CONSTRAINT = 'CK_ArticleCategoryMappings_LeafOnly';
                    END IF;

                    RETURN NEW;
                END;
                $$;

                CREATE TRIGGER "TR_ArticleCategoryMappings_Validate"
                BEFORE INSERT OR UPDATE OF "ArticleCategoryId", "ArticleId"
                ON "ArticleCategoryMappings"
                FOR EACH ROW
                EXECUTE FUNCTION validate_article_category_mapping();
                """);

            migrationBuilder.Sql(
                """
                CREATE FUNCTION validate_article_category_parent()
                RETURNS trigger
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    IF NEW."ParentId" IS NOT NULL
                       AND NOT NEW."IsDeleted"
                       AND EXISTS (
                           SELECT 1
                           FROM "ArticleCategoryMappings"
                           WHERE "ArticleCategoryId" = NEW."ParentId"
                       ) THEN
                        RAISE EXCEPTION 'Danh mục cha đang được gắn trực tiếp với bài viết.'
                            USING ERRCODE = '23514',
                                  CONSTRAINT = 'CK_ArticleCategories_ParentIsNotUsed';
                    END IF;

                    RETURN NEW;
                END;
                $$;

                CREATE TRIGGER "TR_ArticleCategories_ValidateParent"
                BEFORE INSERT OR UPDATE OF "ParentId", "IsDeleted"
                ON "ArticleCategories"
                FOR EACH ROW
                EXECUTE FUNCTION validate_article_category_parent();
                """);

            migrationBuilder.Sql(
                """
                CREATE FUNCTION prevent_article_type_change()
                RETURNS trigger
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    IF NEW."Type" IS DISTINCT FROM OLD."Type"
                       AND EXISTS (
                           SELECT 1
                           FROM "ArticleCategoryMappings"
                           WHERE "ArticleId" = NEW."Id"
                       ) THEN
                        RAISE EXCEPTION 'Không thể đổi nhóm của bài viết đã có danh mục.'
                            USING ERRCODE = '23514',
                                  CONSTRAINT = 'CK_Articles_ImmutableType';
                    END IF;

                    RETURN NEW;
                END;
                $$;

                CREATE TRIGGER "TR_Articles_PreventTypeChange"
                BEFORE UPDATE OF "Type"
                ON "Articles"
                FOR EACH ROW
                EXECUTE FUNCTION prevent_article_type_change();
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TRIGGER IF EXISTS "TR_Articles_PreventTypeChange" ON "Articles";
                DROP FUNCTION IF EXISTS prevent_article_type_change();
                DROP TRIGGER IF EXISTS "TR_ArticleCategories_ValidateParent" ON "ArticleCategories";
                DROP FUNCTION IF EXISTS validate_article_category_parent();
                DROP TRIGGER IF EXISTS "TR_ArticleCategoryMappings_Validate" ON "ArticleCategoryMappings";
                DROP FUNCTION IF EXISTS validate_article_category_mapping();
                """);

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Articles");
        }
    }
}
