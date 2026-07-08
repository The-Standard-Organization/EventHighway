// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEventV2TableToV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventV2s_EventAddressV2s_EventAddressId",
                table: "EventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventV2s_EventId",
                table: "ListenerEventV2s");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventV2s",
                table: "EventV2s");

            migrationBuilder.RenameTable(
                name: "EventV2s",
                newName: "EventV1s");

            migrationBuilder.RenameIndex(
                name: "IX_EventV2s_EventAddressId",
                table: "EventV1s",
                newName: "IX_EventV1s_EventAddressId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventV1s",
                table: "EventV1s",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventV1s_EventAddressV2s_EventAddressId",
                table: "EventV1s",
                column: "EventAddressId",
                principalTable: "EventAddressV2s",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventV1s_EventId",
                table: "ListenerEventV2s",
                column: "EventId",
                principalTable: "EventV1s",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventV1s_EventAddressV2s_EventAddressId",
                table: "EventV1s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventV1s_EventId",
                table: "ListenerEventV2s");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventV1s",
                table: "EventV1s");

            migrationBuilder.RenameTable(
                name: "EventV1s",
                newName: "EventV2s");

            migrationBuilder.RenameIndex(
                name: "IX_EventV1s_EventAddressId",
                table: "EventV2s",
                newName: "IX_EventV2s_EventAddressId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventV2s",
                table: "EventV2s",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventV2s_EventAddressV2s_EventAddressId",
                table: "EventV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV2s",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventV2s_EventId",
                table: "ListenerEventV2s",
                column: "EventId",
                principalTable: "EventV2s",
                principalColumn: "Id");
        }
    }
}
