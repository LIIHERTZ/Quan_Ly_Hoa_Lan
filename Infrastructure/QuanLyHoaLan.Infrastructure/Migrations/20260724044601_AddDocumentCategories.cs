using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyHoaLan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Slug = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    No = table.Column<int>(type: "integer", nullable: false),
                    ConcurrencyStamp = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentCategories_DocumentCategories_ParentId",
                        column: x => x.ParentId,
                        principalTable: "DocumentCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            var researchId = new Guid("10000000-0000-0000-0000-000000000001");
            var journalId = new Guid("10000000-0000-0000-0000-000000000002");
            var generalId = new Guid("10000000-0000-0000-0000-000000000003");
            var createdAt = new DateTime(2026, 7, 24, 0, 0, 0, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                table: "DocumentCategories",
                columns:
                [
                    "Id", "Name", "Description", "Slug", "ParentId", "No",
                    "ConcurrencyStamp", "CreatedAt", "CreatedBy", "UpdatedAt",
                    "UpdatedBy", "IsDeleted", "DeletedAt", "DeletedBy"
                ],
                values: new object[,]
                {
                    {
                        researchId, "Nghiên cứu", "", "nghien-cuu", null, 0,
                        researchId, createdAt, null, null, null, false, null, null
                    },
                    {
                        journalId, "Tạp chí", "", "tap-chi", null, 0,
                        journalId, createdAt, null, null, null, false, null, null
                    },
                    {
                        generalId, "Tài liệu tổng hợp", "", "tai-lieu-tong-hop", null, 0,
                        generalId, createdAt, null, null, null, false, null, null
                    }
                });

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "AppDocuments",
                type: "uuid",
                nullable: false,
                defaultValue: generalId);

            migrationBuilder.CreateIndex(
                name: "IX_AppDocuments_CategoryId",
                table: "AppDocuments",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentCategories_ParentId",
                table: "DocumentCategories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentCategories_Slug",
                table: "DocumentCategories",
                column: "Slug",
                unique: true,
                filter: "\"IsDeleted\" = false");

            migrationBuilder.AddForeignKey(
                name: "FK_AppDocuments_DocumentCategories_CategoryId",
                table: "AppDocuments",
                column: "CategoryId",
                principalTable: "DocumentCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION validate_document_category_leaf()
                RETURNS trigger AS $$
                BEGIN
                    IF NEW."IsDeleted" THEN
                        RETURN NEW;
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

                CREATE OR REPLACE FUNCTION validate_document_category_hierarchy()
                RETURNS trigger AS $$
                BEGIN
                    IF NEW."ParentId" IS NOT NULL THEN
                        IF NEW."ParentId" = NEW."Id" THEN
                            RAISE EXCEPTION 'A document category cannot be its own parent.';
                        END IF;

                        IF NOT EXISTS (
                            SELECT 1
                            FROM "DocumentCategories"
                            WHERE "Id" = NEW."ParentId" AND NOT "IsDeleted"
                        ) THEN
                            RAISE EXCEPTION 'Parent document category does not exist or has been deleted.';
                        END IF;

                        IF EXISTS (
                            SELECT 1
                            FROM "AppDocuments"
                            WHERE "CategoryId" = NEW."ParentId" AND NOT "IsDeleted"
                        ) THEN
                            RAISE EXCEPTION 'A category containing documents cannot become a parent.';
                        END IF;

                        IF EXISTS (
                            WITH RECURSIVE ancestors AS (
                                SELECT "Id", "ParentId"
                                FROM "DocumentCategories"
                                WHERE "Id" = NEW."ParentId" AND NOT "IsDeleted"
                                UNION
                                SELECT parent."Id", parent."ParentId"
                                FROM "DocumentCategories" parent
                                INNER JOIN ancestors child ON parent."Id" = child."ParentId"
                                WHERE NOT parent."IsDeleted"
                            )
                            SELECT 1 FROM ancestors WHERE "Id" = NEW."Id"
                        ) THEN
                            RAISE EXCEPTION 'Document category hierarchy cannot contain a cycle.';
                        END IF;
                    END IF;

                    IF TG_OP = 'UPDATE' AND NEW."IsDeleted" AND NOT OLD."IsDeleted" THEN
                        IF EXISTS (
                            SELECT 1
                            FROM "DocumentCategories"
                            WHERE "ParentId" = NEW."Id" AND NOT "IsDeleted"
                        ) THEN
                            RAISE EXCEPTION 'A category with active children cannot be deleted.';
                        END IF;

                        IF EXISTS (
                            SELECT 1
                            FROM "AppDocuments"
                            WHERE "CategoryId" = NEW."Id" AND NOT "IsDeleted"
                        ) THEN
                            RAISE EXCEPTION 'A category containing active documents cannot be deleted.';
                        END IF;
                    END IF;

                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;

                CREATE TRIGGER trg_validate_document_category_hierarchy
                BEFORE UPDATE OF "ParentId", "IsDeleted"
                ON "DocumentCategories"
                FOR EACH ROW
                EXECUTE FUNCTION validate_document_category_hierarchy();

                CREATE TRIGGER trg_validate_new_document_category_hierarchy
                BEFORE INSERT
                ON "DocumentCategories"
                FOR EACH ROW
                EXECUTE FUNCTION validate_document_category_hierarchy();
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TRIGGER IF EXISTS trg_validate_document_category_leaf ON "AppDocuments";
                DROP TRIGGER IF EXISTS trg_validate_document_category_hierarchy ON "DocumentCategories";
                DROP TRIGGER IF EXISTS trg_validate_new_document_category_hierarchy ON "DocumentCategories";
                DROP FUNCTION IF EXISTS validate_document_category_leaf();
                DROP FUNCTION IF EXISTS validate_document_category_hierarchy();
                """);

            migrationBuilder.DropForeignKey(
                name: "FK_AppDocuments_DocumentCategories_CategoryId",
                table: "AppDocuments");

            migrationBuilder.DropTable(
                name: "DocumentCategories");

            migrationBuilder.DropIndex(
                name: "IX_AppDocuments_CategoryId",
                table: "AppDocuments");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "AppDocuments");
        }
    }
}
