// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddReasonPhraseToListenerEventV1Model : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResponseReasonPhrase",
                table: "ListenerEventV1s",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EventArchiveV1Id",
                table: "ListenerEventArchiveV1s",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV1s_EventArchiveV1Id",
                table: "ListenerEventArchiveV1s",
                column: "EventArchiveV1Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventArchiveV1s_EventArchiveV1s_EventArchiveV1Id",
                table: "ListenerEventArchiveV1s",
                column: "EventArchiveV1Id",
                principalTable: "EventArchiveV1s",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventArchiveV1s_EventArchiveV1s_EventArchiveV1Id",
                table: "ListenerEventArchiveV1s");

            migrationBuilder.DropIndex(
                name: "IX_ListenerEventArchiveV1s_EventArchiveV1Id",
                table: "ListenerEventArchiveV1s");

            migrationBuilder.DropColumn(
                name: "ResponseReasonPhrase",
                table: "ListenerEventV1s");

            migrationBuilder.DropColumn(
                name: "EventArchiveV1Id",
                table: "ListenerEventArchiveV1s");
        }
    }
}
