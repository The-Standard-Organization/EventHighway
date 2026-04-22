using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddV2Models : Migration
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
                name: "EventArchiveV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ScheduledDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ArchivedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EventAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventArchiveV2s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventListenerV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HandlerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EventAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventListenerV2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventListenerV2s_EventAddressV2s_EventAddressId",
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
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "ListenerEventArchiveV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseReasonPhrase = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ArchivedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventListenerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventArchiveV2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListenerEventArchiveV2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListenerEventArchiveV2s_EventArchiveV2s_EventArchiveV2Id",
                        column: x => x.EventArchiveV2Id,
                        principalTable: "EventArchiveV2s",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HandlerConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventListenerV2Id = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HandlerConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HandlerConfigurations_EventListenerV2s_EventListenerV2Id",
                        column: x => x.EventListenerV2Id,
                        principalTable: "EventListenerV2s",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ListenerEventV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseReasonPhrase = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "IX_EventListenerV2s_EventAddressId",
                table: "EventListenerV2s",
                column: "EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_EventV2s_EventAddressId",
                table: "EventV2s",
                column: "EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_HandlerConfigurations_EventListenerV2Id",
                table: "HandlerConfigurations",
                column: "EventListenerV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_EventArchiveV2Id",
                table: "ListenerEventArchiveV2s",
                column: "EventArchiveV2Id");

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
                name: "HandlerConfigurations");

            migrationBuilder.DropTable(
                name: "ListenerEventArchiveV2s");

            migrationBuilder.DropTable(
                name: "ListenerEventV2s");

            migrationBuilder.DropTable(
                name: "EventArchiveV2s");

            migrationBuilder.DropTable(
                name: "EventListenerV2s");

            migrationBuilder.DropTable(
                name: "EventV2s");

            migrationBuilder.DropTable(
                name: "EventAddressV2s");
        }
    }
}
