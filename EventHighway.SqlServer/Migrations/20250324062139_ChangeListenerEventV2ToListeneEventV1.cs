// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class ChangeListenerEventV2ToListeneEventV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListenerEventV2s");

            migrationBuilder.CreateTable(
                name: "ListenerEventV1s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventListenerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListenerEventV1s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListenerEventV1s_EventAddressV1s_EventAddressId",
                        column: x => x.EventAddressId,
                        principalTable: "EventAddressV1s",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ListenerEventV1s_EventListenerV1s_EventListenerId",
                        column: x => x.EventListenerId,
                        principalTable: "EventListenerV1s",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ListenerEventV1s_EventV1s_EventId",
                        column: x => x.EventId,
                        principalTable: "EventV1s",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV1s_EventAddressId",
                table: "ListenerEventV1s",
                column: "EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV1s_EventId",
                table: "ListenerEventV1s",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV1s_EventListenerId",
                table: "ListenerEventV1s",
                column: "EventListenerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListenerEventV1s");

            migrationBuilder.CreateTable(
                name: "ListenerEventV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventListenerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListenerEventV2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListenerEventV2s_EventAddressV1s_EventAddressId",
                        column: x => x.EventAddressId,
                        principalTable: "EventAddressV1s",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ListenerEventV2s_EventListenerV1s_EventListenerId",
                        column: x => x.EventListenerId,
                        principalTable: "EventListenerV1s",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ListenerEventV2s_EventV1s_EventId",
                        column: x => x.EventId,
                        principalTable: "EventV1s",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_EventAddressId",
                table: "ListenerEventV2s",
                column: "EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_EventId",
                table: "ListenerEventV2s",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_EventListenerId",
                table: "ListenerEventV2s",
                column: "EventListenerId");
        }
    }
}
