using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMod.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData_YYYYMMDD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "system_configurations",
                columns: new[] { "Id", "ConfigKey", "ConfigValue", "CreateDate", "DeleteDate", "Description", "IsDeleted", "IsEnabled", "UpdateDate" },
                values: new object[] { 7, "SLUG_STRING_LENGTH", "24", new DateTime(2026, 1, 12, 16, 31, 0, 0, DateTimeKind.Unspecified), null, "文章用于SEO的Slug字符串长度", false, true, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "system_configurations",
                keyColumn: "Id",
                keyValue: 7);
        }
    }
}
