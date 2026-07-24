using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoaLan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDefaultDocumentCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TRIGGER IF EXISTS trg_validate_document_category_leaf ON "AppDocuments";
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "AppDocuments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.Sql(
                """
                UPDATE "AppDocuments"
                SET "CategoryId" = NULL
                WHERE "CategoryId" IN (
                    '10000000-0000-0000-0000-000000000001',
                    '10000000-0000-0000-0000-000000000002',
                    '10000000-0000-0000-0000-000000000003'
                );

                UPDATE "DocumentCategories"
                SET "ParentId" = NULL
                WHERE "ParentId" IN (
                    '10000000-0000-0000-0000-000000000001',
                    '10000000-0000-0000-0000-000000000002',
                    '10000000-0000-0000-0000-000000000003'
                );

                DELETE FROM "DocumentCategories"
                WHERE "Id" IN (
                    '10000000-0000-0000-0000-000000000001',
                    '10000000-0000-0000-0000-000000000002',
                    '10000000-0000-0000-0000-000000000003'
                );

                CREATE OR REPLACE FUNCTION validate_document_category_leaf()
                RETURNS trigger AS $$
                BEGIN
                    IF NEW."IsDeleted" THEN
                        RETURN NEW;
                    END IF;

                    IF NEW."CategoryId" IS NULL THEN
                        RAISE EXCEPTION 'Document category is required.';
                    END IF;

                    IF NOT EXISTS (
                        SELECT 1
                        FROM "DocumentCategories"
                        WHERE "Id" = NEW."CategoryId" AND NOT "IsDeleted"
                    ) THEN
                        RAISE EXCEPTION 'Document category does not exist or has been deleted.';
                    END IF;

                    IF EXISTS (
                        SELECT 1
                        FROM "DocumentCategories"
                        WHERE "ParentId" = NEW."CategoryId" AND NOT "IsDeleted"
                    ) THEN
                        RAISE EXCEPTION 'Documents can only be assigned to leaf categories.';
                    END IF;

                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                CREATE TRIGGER trg_validate_document_category_leaf
                BEFORE INSERT OR UPDATE OF "CategoryId", "IsDeleted"
                ON "AppDocuments"
                FOR EACH ROW
                EXECUTE FUNCTION validate_document_category_leaf();
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Data cleanup is intentionally irreversible: rolling back must not
            // recreate application-owned categories behind the administrator's back.
        }
    }
}
