// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEventAddressV2ToEventAddressV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventListenerV2s_EventAddressV2s_EventAddressId",
                table: "EventListenerV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventV1s_EventAddressV2s_EventAddressId",
                table: "EventV1s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventAddressV2s_EventAddressId",
                table: "ListenerEventV2s");

            migrationBuilder.DropTable(
                name: "EventAddressV2s");

            migrationBuilder.CreateTable(
                name: "EventAddressV1s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAddressV1s", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_EventListenerV2s_EventAddressV1s_EventAddressId",
                table: "EventListenerV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV1s",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventV1s_EventAddressV1s_EventAddressId",
                table: "EventV1s",
                column: "EventAddressId",
                principalTable: "EventAddressV1s",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventAddressV1s_EventAddressId",
                table: "ListenerEventV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV1s",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventListenerV2s_EventAddressV1s_EventAddressId",
                table: "EventListenerV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventV1s_EventAddressV1s_EventAddressId",
                table: "EventV1s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventAddressV1s_EventAddressId",
                table: "ListenerEventV2s");

            migrationBuilder.DropTable(
                name: "EventAddressV1s");

            migrationBuilder.CreateTable(
                name: "EventAddressV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAddressV2s", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_EventListenerV2s_EventAddressV2s_EventAddressId",
                table: "EventListenerV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV2s",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventV1s_EventAddressV2s_EventAddressId",
                table: "EventV1s",
                column: "EventAddressId",
                principalTable: "EventAddressV2s",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventAddressV2s_EventAddressId",
                table: "ListenerEventV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV2s",
                principalColumn: "Id");
        }
    }
}
