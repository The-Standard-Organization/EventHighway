// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddEventAddressV2sTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventV2s_EventAddressV2_EventAddressId",
                table: "EventV2s");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventAddressV2",
                table: "EventAddressV2");

            migrationBuilder.RenameTable(
                name: "EventAddressV2",
                newName: "EventAddressV2s");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventAddressV2s",
                table: "EventAddressV2s",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventV2s_EventAddressV2s_EventAddressId",
                table: "EventV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV2s",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventV2s_EventAddressV2s_EventAddressId",
                table: "EventV2s");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventAddressV2s",
                table: "EventAddressV2s");

            migrationBuilder.RenameTable(
                name: "EventAddressV2s",
                newName: "EventAddressV2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventAddressV2",
                table: "EventAddressV2",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventV2s_EventAddressV2_EventAddressId",
                table: "EventV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV2",
                principalColumn: "Id");
        }
    }
}
