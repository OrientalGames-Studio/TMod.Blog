using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMod.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReposInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsShareEnabled",
                table: "comments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsShareEnabled",
                table: "comments");
        }
    }
}
