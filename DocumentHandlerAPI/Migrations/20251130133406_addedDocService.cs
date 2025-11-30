using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DocumentHandlerAPI.Migrations
{
    /// <inheritdoc />
    public partial class addedDocService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(26)", maxLength: 26, nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    URL = table.Column<string>(type: "text", nullable: false),
                    DocumentType = table.Column<string>(type: "text", nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(type: "timestamptz", nullable: false),
                    LastModificationTime = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    DeletionTime = table.Column<DateTimeOffset>(type: "timestamptz", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");
        }
    }
}
