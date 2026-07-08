using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddEventArchiveV2ForeignKeys : Migration
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
