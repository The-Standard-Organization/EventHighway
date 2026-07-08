// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEventListenerV2ToEventListenerV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventListenerV2s_EventListenerId",
                table: "ListenerEventV2s");

            migrationBuilder.DropTable(
                name: "EventListenerV2s");

            migrationBuilder.CreateTable(
                name: "EventListenerV1s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeaderSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EventAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventListenerV1s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventListenerV1s_EventAddressV1s_EventAddressId",
                        column: x => x.EventAddressId,
                        principalTable: "EventAddressV1s",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventListenerV1s_EventAddressId",
                table: "EventListenerV1s",
                column: "EventAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventListenerV1s_EventListenerId",
                table: "ListenerEventV2s",
                column: "EventListenerId",
                principalTable: "EventListenerV1s",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventListenerV1s_EventListenerId",
                table: "ListenerEventV2s");

            migrationBuilder.DropTable(
                name: "EventListenerV1s");

            migrationBuilder.CreateTable(
                name: "EventListenerV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeaderSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventListenerV2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventListenerV2s_EventAddressV1s_EventAddressId",
                        column: x => x.EventAddressId,
                        principalTable: "EventAddressV1s",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventListenerV2s_EventAddressId",
                table: "EventListenerV2s",
                column: "EventAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventListenerV2s_EventListenerId",
                table: "ListenerEventV2s",
                column: "EventListenerId",
                principalTable: "EventListenerV2s",
                principalColumn: "Id");
        }
    }
}
