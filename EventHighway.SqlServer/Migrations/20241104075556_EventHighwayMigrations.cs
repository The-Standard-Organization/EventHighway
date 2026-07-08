// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class EventHighwayMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventAddresses",
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
                    table.PrimaryKey("PK_EventAddresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventListeners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventListeners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventListeners_EventAddresses_EventAddressId",
                        column: x => x.EventAddressId,
                        principalTable: "EventAddresses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_EventAddresses_EventAddressId",
                        column: x => x.EventAddressId,
                        principalTable: "EventAddresses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ListenerEvents",
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
                    table.PrimaryKey("PK_ListenerEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListenerEvents_EventAddresses_EventAddressId",
                        column: x => x.EventAddressId,
                        principalTable: "EventAddresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ListenerEvents_EventListeners_EventListenerId",
                        column: x => x.EventListenerId,
                        principalTable: "EventListeners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ListenerEvents_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventListeners_EventAddressId",
                table: "EventListeners",
                column: "EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventAddressId",
                table: "Events",
                column: "EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEvents_EventAddressId",
                table: "ListenerEvents",
                column: "EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEvents_EventId",
                table: "ListenerEvents",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEvents_EventListenerId",
                table: "ListenerEvents",
                column: "EventListenerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListenerEvents");

            migrationBuilder.DropTable(
                name: "EventListeners");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "EventAddresses");
        }
    }
}
