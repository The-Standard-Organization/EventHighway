using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class RenameEventV1ArchireToEventArchiveV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV1Archives_EventV1Archives_EventV1ArchiveId",
                table: "ListenerEventV1Archives");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ListenerEventV1Archives",
                table: "ListenerEventV1Archives");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventV1Archives",
                table: "EventV1Archives");

            migrationBuilder.RenameTable(
                name: "EventV1Archives",
                newName: "EventArchiveV1s");

            migrationBuilder.RenameTable(
                name: "ListenerEventV1Archives",
                newName: "ListenerEventArchiveV1s");

            migrationBuilder.RenameIndex(
                name: "IX_ListenerEventV1Archives_EventV1ArchiveId",
                table: "ListenerEventArchiveV1s",
                newName: "IX_ListenerEventArchiveV1s_EventArchiveV1Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventArchiveV1s",
                table: "EventArchiveV1s",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListenerEventArchiveV1s",
                table: "ListenerEventArchiveV1s",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ListenerEventArchiveV1s",
                table: "ListenerEventArchiveV1s");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventArchiveV1s",
                table: "EventArchiveV1s");

            migrationBuilder.RenameTable(
                name: "EventArchiveV1s",
                newName: "EventV1Archives");

            migrationBuilder.RenameTable(
                name: "ListenerEventArchiveV1s",
                newName: "ListenerEventV1Archives");

            migrationBuilder.RenameIndex(
                name: "IX_ListenerEventArchiveV1s_EventArchiveV1Id",
                table: "ListenerEventV1Archives",
                newName: "IX_ListenerEventV1Archives_EventV1ArchiveId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventV1Archives",
                table: "EventV1Archives",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ListenerEventV1Archives",
                table: "ListenerEventV1Archives",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV1Archives_EventV1Archives_EventV1ArchiveId",
                table: "ListenerEventV1Archives",
                column: "EventV1ArchiveId",
                principalTable: "EventV1Archives",
                principalColumn: "Id");
        }
    }
}