using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TMod.Blog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoritesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "favorites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Fingerprint = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientIp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FavoriteType = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdateDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeleteDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_favorites", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "system_configurations",
                columns: new[] { "Id", "ConfigKey", "ConfigValue", "CreateDate", "DeleteDate", "Description", "IsDeleted", "IsEnabled", "UpdateDate" },
                values: new object[,]
                {
                    { 2, "SITE_SHORT_CODE_WORKER_ID", "522", new DateTime(2025, 11, 7, 17, 40, 0, 0, DateTimeKind.Unspecified), null, "短码服务的 Worker Id", false, true, null },
                    { 3, "SITE_SHORT_CODE_EPOCH", "2025-01-01", new DateTime(2025, 11, 7, 17, 40, 0, 0, DateTimeKind.Unspecified), null, "短码服务的 epoch", false, true, null },
                    { 4, "SITE_SHORT_CODE_SECRET_KEY", "ZyfPassw0rd!Blog", new DateTime(2025, 11, 7, 17, 40, 0, 0, DateTimeKind.Unspecified), null, "短码服务的密钥", false, true, null },
                    { 5, "SITE_SHORT_CODE_MIN_LENGTH", "6", new DateTime(2025, 11, 7, 17, 40, 0, 0, DateTimeKind.Unspecified), null, "短码服务生成的短码最少几位", false, true, null },
                    { 6, "SITE_SHORT_CODE_MAX_LENGTH", "20", new DateTime(2025, 11, 7, 17, 40, 0, 0, DateTimeKind.Unspecified), null, "短码服务生成的短码最多几位", false, true, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "favorites");

            migrationBuilder.DeleteData(
                table: "system_configurations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "system_configurations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "system_configurations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "system_configurations",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "system_configurations",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
