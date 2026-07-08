using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class RestrictParticipantAddressListenerDeletes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventArchiveV2s_EventParticipantV2s_ParticipantId",
                table: "EventArchiveV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventListenerV2s_EventAddressV2s_EventAddressId",
                table: "EventListenerV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventListenerV2s_EventParticipantV2s_ParticipantId",
                table: "EventListenerV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventV2s_EventAddressV2s_EventAddressId",
                table: "EventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventV2s_EventParticipantV2s_ParticipantId",
                table: "EventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventParticipantV2s_ParticipantId",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventAddressV2s_EventAddressId",
                table: "ListenerEventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventListenerV2s_EventListenerId",
                table: "ListenerEventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventParticipantV2s_ParticipantId",
                table: "ListenerEventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventV2s_EventId",
                table: "ListenerEventV2s");

            migrationBuilder.AddForeignKey(
                name: "FK_EventArchiveV2s_EventParticipantV2s_ParticipantId",
                table: "EventArchiveV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventListenerV2s_EventAddressV2s_EventAddressId",
                table: "EventListenerV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventListenerV2s_EventParticipantV2s_ParticipantId",
                table: "EventListenerV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventV2s_EventAddressV2s_EventAddressId",
                table: "EventV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventV2s_EventParticipantV2s_ParticipantId",
                table: "EventV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventParticipantV2s_ParticipantId",
                table: "ListenerEventArchiveV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventAddressV2s_EventAddressId",
                table: "ListenerEventV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventListenerV2s_EventListenerId",
                table: "ListenerEventV2s",
                column: "EventListenerId",
                principalTable: "EventListenerV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventParticipantV2s_ParticipantId",
                table: "ListenerEventV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventV2s_EventId",
                table: "ListenerEventV2s",
                column: "EventId",
                principalTable: "EventV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventArchiveV2s_EventParticipantV2s_ParticipantId",
                table: "EventArchiveV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventListenerV2s_EventAddressV2s_EventAddressId",
                table: "EventListenerV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventListenerV2s_EventParticipantV2s_ParticipantId",
                table: "EventListenerV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventV2s_EventAddressV2s_EventAddressId",
                table: "EventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventV2s_EventParticipantV2s_ParticipantId",
                table: "EventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventParticipantV2s_ParticipantId",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventAddressV2s_EventAddressId",
                table: "ListenerEventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventListenerV2s_EventListenerId",
                table: "ListenerEventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventParticipantV2s_ParticipantId",
                table: "ListenerEventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventV2s_EventId",
                table: "ListenerEventV2s");

            migrationBuilder.AddForeignKey(
                name: "FK_EventArchiveV2s_EventParticipantV2s_ParticipantId",
                table: "EventArchiveV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EventListenerV2s_EventAddressV2s_EventAddressId",
                table: "EventListenerV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV2s",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventListenerV2s_EventParticipantV2s_ParticipantId",
                table: "EventListenerV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EventV2s_EventAddressV2s_EventAddressId",
                table: "EventV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV2s",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventV2s_EventParticipantV2s_ParticipantId",
                table: "EventV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventParticipantV2s_ParticipantId",
                table: "ListenerEventArchiveV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventAddressV2s_EventAddressId",
                table: "ListenerEventV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV2s",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventListenerV2s_EventListenerId",
                table: "ListenerEventV2s",
                column: "EventListenerId",
                principalTable: "EventListenerV2s",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventParticipantV2s_ParticipantId",
                table: "ListenerEventV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventV2s_EventId",
                table: "ListenerEventV2s",
                column: "EventId",
                principalTable: "EventV2s",
                principalColumn: "Id");
        }
    }
}
