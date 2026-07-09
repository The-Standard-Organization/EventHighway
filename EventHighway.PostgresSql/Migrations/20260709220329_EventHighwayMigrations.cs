using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.PostgresSql.Migrations
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAddresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventAddressV1s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAddressV1s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventAddressV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAddressV2s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventParticipantV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ContactEmail = table.Column<string>(type: "text", nullable: true),
                    ContactPhone = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: true),
                    ActiveTo = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipantV2s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventV1Archives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    ScheduledDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: true),
                    ArchivedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    EventAddressId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventV1Archives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventListeners",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    HeaderSecret = table.Column<string>(type: "text", nullable: true),
                    Endpoint = table.Column<string>(type: "text", nullable: true),
                    EventAddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    EventAddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false)
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
                name: "EventListenerV1s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    HeaderSecret = table.Column<string>(type: "text", nullable: true),
                    Endpoint = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    EventAddressId = table.Column<Guid>(type: "uuid", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "EventV1s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    ScheduledDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: true),
                    RetryAttempts = table.Column<int>(type: "integer", nullable: false),
                    EventAddressId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventV1s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventV1s_EventAddressV1s_EventAddressId",
                        column: x => x.EventAddressId,
                        principalTable: "EventAddressV1s",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventArchiveV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    EventName = table.Column<string>(type: "text", nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    ScheduledDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: true),
                    ArchivedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    EventAddressV2Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventParticipantV2Secret = table.Column<string>(type: "text", nullable: true),
                    EventParticipantV2Id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventArchiveV2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventArchiveV2s_EventAddressV2s_EventAddressV2Id",
                        column: x => x.EventAddressV2Id,
                        principalTable: "EventAddressV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventArchiveV2s_EventParticipantV2s_EventParticipantV2Id",
                        column: x => x.EventParticipantV2Id,
                        principalTable: "EventParticipantV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventListenerV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    HandlerId = table.Column<Guid>(type: "uuid", nullable: false),
                    HandlerName = table.Column<string>(type: "text", nullable: true),
                    PromotedProperties = table.Column<string>(type: "text", nullable: true),
                    FilterCriteria = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    EventAddressV2Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventParticipantV2Id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventListenerV2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventListenerV2s_EventAddressV2s_EventAddressV2Id",
                        column: x => x.EventAddressV2Id,
                        principalTable: "EventAddressV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventListenerV2s_EventParticipantV2s_EventParticipantV2Id",
                        column: x => x.EventParticipantV2Id,
                        principalTable: "EventParticipantV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EventParticipantSecretV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Secret = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: true),
                    ActiveTo = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    EventParticipantV2Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipantSecretV2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventParticipantSecretV2s_EventParticipantV2s_EventParticip~",
                        column: x => x.EventParticipantV2Id,
                        principalTable: "EventParticipantV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    EventName = table.Column<string>(type: "text", nullable: true),
                    ContentHash = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    ScheduledDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: true),
                    EventAddressV2Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventParticipantV2Secret = table.Column<string>(type: "text", nullable: true),
                    EventParticipantV2Id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventV2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventV2s_EventAddressV2s_EventAddressV2Id",
                        column: x => x.EventAddressV2Id,
                        principalTable: "EventAddressV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventV2s_EventParticipantV2s_EventParticipantV2Id",
                        column: x => x.EventParticipantV2Id,
                        principalTable: "EventParticipantV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ListenerEventV1Archives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Response = table.Column<string>(type: "text", nullable: true),
                    ResponseReasonPhrase = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    ArchivedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventAddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventListenerId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventArchiveV1Id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListenerEventV1Archives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListenerEventV1Archives_EventV1Archives_EventArchiveV1Id",
                        column: x => x.EventArchiveV1Id,
                        principalTable: "EventV1Archives",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ListenerEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Response = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventAddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventListenerId = table.Column<Guid>(type: "uuid", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ListenerEventV1s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Response = table.Column<string>(type: "text", nullable: true),
                    ResponseReasonPhrase = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    EventId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventAddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventListenerId = table.Column<Guid>(type: "uuid", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "ListenerEventArchiveV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Response = table.Column<string>(type: "text", nullable: true),
                    ResponseCode = table.Column<string>(type: "text", nullable: true),
                    ResponseMessage = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    RemainingRetryAttempts = table.Column<int>(type: "integer", nullable: false),
                    RetryAttemptsAllowed = table.Column<int>(type: "integer", nullable: false),
                    NextRetryAttemptNotBefore = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: true),
                    DispatchedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: true),
                    ArchivedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    EventV2Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventAddressV2Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventListenerV2Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventArchiveV2Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventParticipantV2Id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListenerEventArchiveV2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListenerEventArchiveV2s_EventArchiveV2s_EventArchiveV2Id",
                        column: x => x.EventArchiveV2Id,
                        principalTable: "EventArchiveV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListenerEventArchiveV2s_EventListenerV2s_EventListenerV2Id",
                        column: x => x.EventListenerV2Id,
                        principalTable: "EventListenerV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListenerEventArchiveV2s_EventParticipantV2s_EventParticipan~",
                        column: x => x.EventParticipantV2Id,
                        principalTable: "EventParticipantV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ListenerEventV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CorrelationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Response = table.Column<string>(type: "text", nullable: true),
                    ResponseCode = table.Column<string>(type: "text", nullable: true),
                    ResponseMessage = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: false),
                    RemainingRetryAttempts = table.Column<int>(type: "integer", nullable: false),
                    RetryAttemptsAllowed = table.Column<int>(type: "integer", nullable: false),
                    NextRetryAttemptNotBefore = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: true),
                    DispatchedDate = table.Column<DateTimeOffset>(type: "timestamp(6) with time zone", precision: 6, nullable: true),
                    EventV2Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventAddressV2Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventListenerV2Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EventParticipantV2Id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListenerEventV2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListenerEventV2s_EventAddressV2s_EventAddressV2Id",
                        column: x => x.EventAddressV2Id,
                        principalTable: "EventAddressV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListenerEventV2s_EventListenerV2s_EventListenerV2Id",
                        column: x => x.EventListenerV2Id,
                        principalTable: "EventListenerV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListenerEventV2s_EventParticipantV2s_EventParticipantV2Id",
                        column: x => x.EventParticipantV2Id,
                        principalTable: "EventParticipantV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListenerEventV2s_EventV2s_EventV2Id",
                        column: x => x.EventV2Id,
                        principalTable: "EventV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventAddressV2s_Name",
                table: "EventAddressV2s",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventArchiveV2s_AddressArchivedDate",
                table: "EventArchiveV2s",
                columns: new[] { "EventAddressV2Id", "ArchivedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EventArchiveV2s_ArchivedDate",
                table: "EventArchiveV2s",
                column: "ArchivedDate");

            migrationBuilder.CreateIndex(
                name: "IX_EventArchiveV2s_ContentHash",
                table: "EventArchiveV2s",
                columns: new[] { "EventAddressV2Id", "ContentHash" });

            migrationBuilder.CreateIndex(
                name: "IX_EventArchiveV2s_EventParticipantV2Id",
                table: "EventArchiveV2s",
                column: "EventParticipantV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_EventArchiveV2s_Status",
                table: "EventArchiveV2s",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EventListeners_EventAddressId",
                table: "EventListeners",
                column: "EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_EventListenerV1s_EventAddressId",
                table: "EventListenerV1s",
                column: "EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_EventListenerV2s_EventAddressV2Id",
                table: "EventListenerV2s",
                column: "EventAddressV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_EventListenerV2s_EventParticipantV2Id",
                table: "EventListenerV2s",
                column: "EventParticipantV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_EventListenerV2s_Name",
                table: "EventListenerV2s",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipantSecretV2s_EventParticipantV2Id",
                table: "EventParticipantSecretV2s",
                column: "EventParticipantV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipantSecretV2s_Id_Secret",
                table: "EventParticipantSecretV2s",
                columns: new[] { "Id", "Secret" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventAddressId",
                table: "Events",
                column: "EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_EventV1s_EventAddressId",
                table: "EventV1s",
                column: "EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_EventV2s_AddressCreatedDate",
                table: "EventV2s",
                columns: new[] { "EventAddressV2Id", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EventV2s_CreatedDate",
                table: "EventV2s",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_EventV2s_EventParticipantV2Id",
                table: "EventV2s",
                column: "EventParticipantV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_EventV2s_LoopDetection",
                table: "EventV2s",
                columns: new[] { "EventAddressV2Id", "EventName", "ContentHash", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EventV2s_StatusType",
                table: "EventV2s",
                columns: new[] { "Status", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_AddressArchivedDate",
                table: "ListenerEventArchiveV2s",
                columns: new[] { "EventAddressV2Id", "ArchivedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_ArchivedDate",
                table: "ListenerEventArchiveV2s",
                column: "ArchivedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_EventArchiveV2Id",
                table: "ListenerEventArchiveV2s",
                column: "EventArchiveV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_EventListenerV2Id",
                table: "ListenerEventArchiveV2s",
                column: "EventListenerV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_EventParticipantV2Id",
                table: "ListenerEventArchiveV2s",
                column: "EventParticipantV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_RetryClassification",
                table: "ListenerEventArchiveV2s",
                columns: new[] { "Status", "RemainingRetryAttempts" });

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_Status",
                table: "ListenerEventArchiveV2s",
                column: "Status");

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

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV1Archives_EventArchiveV1Id",
                table: "ListenerEventV1Archives",
                column: "EventArchiveV1Id");

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

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_AddressCreatedDate",
                table: "ListenerEventV2s",
                columns: new[] { "EventAddressV2Id", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_CreatedDate",
                table: "ListenerEventV2s",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_EventListenerV2Id",
                table: "ListenerEventV2s",
                column: "EventListenerV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_EventParticipantV2Id",
                table: "ListenerEventV2s",
                column: "EventParticipantV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_RetryClassification",
                table: "ListenerEventV2s",
                columns: new[] { "Status", "RemainingRetryAttempts" });

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_ToBeArchived",
                table: "ListenerEventV2s",
                column: "EventV2Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventParticipantSecretV2s");

            migrationBuilder.DropTable(
                name: "ListenerEventArchiveV2s");

            migrationBuilder.DropTable(
                name: "ListenerEvents");

            migrationBuilder.DropTable(
                name: "ListenerEventV1Archives");

            migrationBuilder.DropTable(
                name: "ListenerEventV1s");

            migrationBuilder.DropTable(
                name: "ListenerEventV2s");

            migrationBuilder.DropTable(
                name: "EventArchiveV2s");

            migrationBuilder.DropTable(
                name: "EventListeners");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "EventV1Archives");

            migrationBuilder.DropTable(
                name: "EventListenerV1s");

            migrationBuilder.DropTable(
                name: "EventV1s");

            migrationBuilder.DropTable(
                name: "EventListenerV2s");

            migrationBuilder.DropTable(
                name: "EventV2s");

            migrationBuilder.DropTable(
                name: "EventAddresses");

            migrationBuilder.DropTable(
                name: "EventAddressV1s");

            migrationBuilder.DropTable(
                name: "EventAddressV2s");

            migrationBuilder.DropTable(
                name: "EventParticipantV2s");
        }
    }
}
