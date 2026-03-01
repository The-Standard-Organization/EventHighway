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
                name: "EventV1ArchiveId",
                table: "ListenerEventV1Archives",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV1Archives_EventV1ArchiveId",
                table: "ListenerEventV1Archives",
                column: "EventV1ArchiveId");

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV1Archives_EventV1Archives_EventV1ArchiveId",
                table: "ListenerEventV1Archives",
                column: "EventV1ArchiveId",
                principalTable: "EventV1Archives",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV1Archives_EventV1Archives_EventV1ArchiveId",
                table: "ListenerEventV1Archives");

            migrationBuilder.DropIndex(
                name: "IX_ListenerEventV1Archives_EventV1ArchiveId",
                table: "ListenerEventV1Archives");

            migrationBuilder.DropColumn(
                name: "ResponseReasonPhrase",
                table: "ListenerEventV1s");

            migrationBuilder.DropColumn(
                name: "EventV1ArchiveId",
                table: "ListenerEventV1Archives");
        }
    }
}
