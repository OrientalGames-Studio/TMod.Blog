using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMod.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameArticles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                newName: "articles");

            migrationBuilder.RenameIndex(
                name: "IX_article_Slug",
                table: "articles",
                newName: "IX_articles_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_article_CategoryId",
                table: "articles",
                newName: "IX_articles_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_articles",
                table: "articles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_article_tags_articles_ArticleId",
                table: "article_tags",
                column: "ArticleId",
                principalTable: "articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_articles_categories_CategoryId",
                table: "articles",
                column: "CategoryId",
                principalTable: "categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_comments_articles_ArticleId",
                table: "comments",
                column: "ArticleId",
                principalTable: "articles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_article_tags_articles_ArticleId",
                table: "article_tags");

            migrationBuilder.DropForeignKey(
                name: "FK_articles_categories_CategoryId",
                table: "articles");

            migrationBuilder.DropForeignKey(
                name: "FK_comments_articles_ArticleId",
                table: "comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_articles",
                table: "articles");

            migrationBuilder.RenameTable(
                name: "articles",
                newName: "article");

            migrationBuilder.RenameIndex(
                name: "IX_articles_Slug",
                table: "article",
                newName: "IX_article_Slug");

            migrationBuilder.RenameIndex(
                name: "IX_articles_CategoryId",
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
    }
}
