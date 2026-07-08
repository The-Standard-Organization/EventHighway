// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddMigrateListenerEventV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListenerEventV2s",
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
                    table.PrimaryKey("PK_ListenerEventV2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListenerEventV2s_EventAddressV2s_EventAddressId",
                        column: x => x.EventAddressId,
                        principalTable: "EventAddressV2s",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ListenerEventV2s_EventListenerV2s_EventListenerId",
                        column: x => x.EventListenerId,
                        principalTable: "EventListenerV2s",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ListenerEventV2s_EventV2s_EventId",
                        column: x => x.EventId,
                        principalTable: "EventV2s",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListenerEventV2s");
        }
    }
}
