using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddEventListenerV2Models : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventAddressV2s",
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
                    table.PrimaryKey("PK_EventAddressV2s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventListenersV2",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HandlerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HandlerParamsJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EventAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventListenersV2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventListenersV2_EventAddressV2s_EventAddressId",
                        column: x => x.EventAddressId,
                        principalTable: "EventAddressV2s",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ScheduledDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    EventAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventV2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventV2s_EventAddressV2s_EventAddressId",
                        column: x => x.EventAddressId,
                        principalTable: "EventAddressV2s",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ListenerEventsV2",
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
                    table.PrimaryKey("PK_ListenerEventsV2", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListenerEventsV2_EventAddressV2s_EventAddressId",
                        column: x => x.EventAddressId,
                        principalTable: "EventAddressV2s",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ListenerEventsV2_EventListenersV2_EventListenerId",
                        column: x => x.EventListenerId,
                        principalTable: "EventListenersV2",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ListenerEventsV2_EventV2s_EventId",
                        column: x => x.EventId,
                        principalTable: "EventV2s",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventListenersV2_EventAddressId",
                table: "EventListenersV2",
                column: "EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_EventV2s_EventAddressId",
                table: "EventV2s",
                column: "EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventsV2_EventAddressId",
                table: "ListenerEventsV2",
                column: "EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventsV2_EventId",
                table: "ListenerEventsV2",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventsV2_EventListenerId",
                table: "ListenerEventsV2",
                column: "EventListenerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListenerEventsV2");

            migrationBuilder.DropTable(
                name: "EventListenersV2");

            migrationBuilder.DropTable(
                name: "EventV2s");

            migrationBuilder.DropTable(
                name: "EventAddressV2s");
        }
    }
}
