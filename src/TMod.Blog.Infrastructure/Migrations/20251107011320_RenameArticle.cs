using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMod.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_article_tags_Articles_ArticleId",
                table: "article_tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Articles_categories_CategoryId",
                table: "Articles");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_Articles_ArticleId",
                table: "comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Articles",
                table: "Articles");

            migrationBuilder.RenameTable(
                name: "Articles",
                newName: "article");

            migrationBuilder.RenameIndex(
                name: "IX_Articles_Slug",
                table: "article",
                newName: "IX_article_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_Articles_CategoryId",
                table: "article",
                newName: "IX_article_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_article",
                table: "article",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_article_categories_CategoryId",
                table: "article",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_article_tags_article_ArticleId",
                table: "article_tags",
                column: "ArticleId",
                principalTable: "article",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_article_ArticleId",
                table: "comments",
                column: "ArticleId",
                principalTable: "article",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_article_categories_CategoryId",
                table: "article");

            migrationBuilder.DropForeignKey(
                name: "FK_article_tags_article_ArticleId",
                table: "article_tags");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_article_ArticleId",
                table: "comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_article",
                table: "article");

            migrationBuilder.RenameTable(
                name: "article",
                newName: "Articles");

            migrationBuilder.RenameIndex(
                name: "IX_article_Slug",
                table: "Articles",
                newName: "IX_Articles_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_article_CategoryId",
                table: "Articles",
                newName: "IX_Articles_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Articles",
                table: "Articles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_article_tags_Articles_ArticleId",
                table: "article_tags",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_categories_CategoryId",
                table: "Articles",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_Articles_ArticleId",
                table: "comments",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
