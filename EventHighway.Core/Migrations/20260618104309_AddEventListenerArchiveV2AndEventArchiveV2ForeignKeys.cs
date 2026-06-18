using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddEventListenerArchiveV2AndEventArchiveV2ForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventArchiveV2s_EventArchiveV2Id",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.AlterColumn<Guid>(
                name: "EventArchiveV2Id",
                table: "ListenerEventArchiveV2s",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "EventListenerArchiveV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HandlerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HandlerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PromotedProperties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilterCriteria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ArchivedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EventListenerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventArchiveV2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventListenerArchiveV2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventListenerArchiveV2s_EventArchiveV2s_EventArchiveV2Id",
                        column: x => x.EventArchiveV2Id,
                        principalTable: "EventArchiveV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventListenerArchiveV2s_EventArchiveV2Id",
                table: "EventListenerArchiveV2s",
                column: "EventArchiveV2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventArchiveV2s_EventArchiveV2Id",
                table: "ListenerEventArchiveV2s",
                column: "EventArchiveV2Id",
                principalTable: "EventArchiveV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventArchiveV2s_EventArchiveV2Id",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropTable(
                name: "EventListenerArchiveV2s");

            migrationBuilder.AlterColumn<Guid>(
                name: "EventArchiveV2Id",
                table: "ListenerEventArchiveV2s",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventArchiveV2s_EventArchiveV2Id",
                table: "ListenerEventArchiveV2s",
                column: "EventArchiveV2Id",
                principalTable: "EventArchiveV2s",
                principalColumn: "Id");
        }
    }
}
