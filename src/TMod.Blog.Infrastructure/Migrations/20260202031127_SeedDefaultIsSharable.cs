using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMod.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDefaultIsSharable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "system_configurations",
                columns: new[] { "Id", "ConfigKey", "ConfigValue", "CreateDate", "DeleteDate", "Description", "IsDeleted", "IsEnabled", "UpdateDate" },
                values: new object[] { 8, "SITE_IS_SHARE_ENABLE", "true", new DateTime(2026, 2, 2, 11, 10, 0, 0, DateTimeKind.Unspecified), null, "是否允许分享文章和评论", false, true, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "system_configurations",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
