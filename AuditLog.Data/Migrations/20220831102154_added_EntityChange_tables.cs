using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLog.Data.Migrations
{
    public partial class added_EntityChange_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EntityChangeSets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrowserInfo = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ClientIpAddress = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    ClientName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtensionData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImpersonatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityChangeSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityChanges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChangeTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangeType = table.Column<byte>(type: "tinyint", nullable: false),
                    EntityChangeSetId = table.Column<long>(type: "bigint", nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(48)", maxLength: 48, nullable: false),
                    EntityTypeFullName = table.Column<string>(type: "nvarchar(192)", maxLength: 192, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityChanges_EntityChangeSets_EntityChangeSetId",
                        column: x => x.EntityChangeSetId,
                        principalTable: "EntityChangeSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EntityPropertyChanges",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityChangeId = table.Column<long>(type: "bigint", nullable: false),
                    NewValue = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    OriginalValue = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(96)", maxLength: 96, nullable: false),
                    PropertyTypeFullName = table.Column<string>(type: "nvarchar(192)", maxLength: 192, nullable: false),
                    NewValueHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginalValueHash = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityPropertyChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntityPropertyChanges_EntityChanges_EntityChangeId",
                        column: x => x.EntityChangeId,
                        principalTable: "EntityChanges",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EntityChanges_EntityChangeSetId",
                table: "EntityChanges",
                column: "EntityChangeSetId");

            migrationBuilder.CreateIndex(
                name: "IX_EntityPropertyChanges_EntityChangeId",
                table: "EntityPropertyChanges",
                column: "EntityChangeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntityPropertyChanges");

            migrationBuilder.DropTable(
                name: "EntityChanges");

            migrationBuilder.DropTable(
                name: "EntityChangeSets");
        }
    }
}
