using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class RenameV2ForeignKeyPropertiesAndCascadeEventV2ListenerEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventArchiveV2s_EventAddressV2s_EventAddressId",
                table: "EventArchiveV2s");

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
                name: "FK_EventParticipantSecretV2s_EventParticipantV2s_ParticipantId",
                table: "EventParticipantSecretV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventV2s_EventAddressV2s_EventAddressId",
                table: "EventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventV2s_EventParticipantV2s_ParticipantId",
                table: "EventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventListenerV2s_EventListenerId",
                table: "ListenerEventArchiveV2s");

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

            migrationBuilder.DropIndex(
                name: "IX_ListenerEventArchiveV2s_EventListenerId",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "ListenerEventV2s",
                newName: "EventParticipantV2Id");

            migrationBuilder.RenameColumn(
                name: "EventListenerId",
                table: "ListenerEventV2s",
                newName: "EventListenerV2Id");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "ListenerEventV2s",
                newName: "EventV2Id");

            migrationBuilder.RenameColumn(
                name: "EventAddressId",
                table: "ListenerEventV2s",
                newName: "EventAddressV2Id");

            migrationBuilder.RenameIndex(
                name: "IX_ListenerEventV2s_ParticipantId",
                table: "ListenerEventV2s",
                newName: "IX_ListenerEventV2s_EventParticipantV2Id");

            migrationBuilder.RenameIndex(
                name: "IX_ListenerEventV2s_EventListenerId",
                table: "ListenerEventV2s",
                newName: "IX_ListenerEventV2s_EventListenerV2Id");

            migrationBuilder.RenameIndex(
                name: "IX_ListenerEventV2s_EventId",
                table: "ListenerEventV2s",
                newName: "IX_ListenerEventV2s_EventV2Id");

            migrationBuilder.RenameIndex(
                name: "IX_ListenerEventV2s_EventAddressId",
                table: "ListenerEventV2s",
                newName: "IX_ListenerEventV2s_EventAddressV2Id");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "ListenerEventArchiveV2s",
                newName: "EventParticipantV2Id");

            migrationBuilder.RenameColumn(
                name: "EventListenerId",
                table: "ListenerEventArchiveV2s",
                newName: "EventListenerV2Id");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "ListenerEventArchiveV2s",
                newName: "EventV2Id");

            migrationBuilder.RenameColumn(
                name: "EventAddressId",
                table: "ListenerEventArchiveV2s",
                newName: "EventAddressV2Id");

            migrationBuilder.RenameIndex(
                name: "IX_ListenerEventArchiveV2s_ParticipantId",
                table: "ListenerEventArchiveV2s",
                newName: "IX_ListenerEventArchiveV2s_EventParticipantV2Id");

            migrationBuilder.RenameColumn(
                name: "ParticipantSecret",
                table: "EventV2s",
                newName: "EventParticipantV2Secret");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "EventV2s",
                newName: "EventParticipantV2Id");

            migrationBuilder.RenameColumn(
                name: "EventAddressId",
                table: "EventV2s",
                newName: "EventAddressV2Id");

            migrationBuilder.RenameIndex(
                name: "IX_EventV2s_ParticipantId",
                table: "EventV2s",
                newName: "IX_EventV2s_EventParticipantV2Id");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "EventParticipantSecretV2s",
                newName: "EventParticipantV2Id");

            migrationBuilder.RenameIndex(
                name: "IX_EventParticipantSecretV2s_ParticipantId",
                table: "EventParticipantSecretV2s",
                newName: "IX_EventParticipantSecretV2s_EventParticipantV2Id");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "EventListenerV2s",
                newName: "EventParticipantV2Id");

            migrationBuilder.RenameColumn(
                name: "EventAddressId",
                table: "EventListenerV2s",
                newName: "EventAddressV2Id");

            migrationBuilder.RenameIndex(
                name: "IX_EventListenerV2s_ParticipantId",
                table: "EventListenerV2s",
                newName: "IX_EventListenerV2s_EventParticipantV2Id");

            migrationBuilder.RenameIndex(
                name: "IX_EventListenerV2s_EventAddressId",
                table: "EventListenerV2s",
                newName: "IX_EventListenerV2s_EventAddressV2Id");

            migrationBuilder.RenameColumn(
                name: "ParticipantSecret",
                table: "EventArchiveV2s",
                newName: "EventParticipantV2Secret");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "EventArchiveV2s",
                newName: "EventParticipantV2Id");

            migrationBuilder.RenameColumn(
                name: "EventAddressId",
                table: "EventArchiveV2s",
                newName: "EventAddressV2Id");

            migrationBuilder.RenameIndex(
                name: "IX_EventArchiveV2s_ParticipantId",
                table: "EventArchiveV2s",
                newName: "IX_EventArchiveV2s_EventParticipantV2Id");

            migrationBuilder.RenameIndex(
                name: "IX_EventArchiveV2s_EventAddressId",
                table: "EventArchiveV2s",
                newName: "IX_EventArchiveV2s_EventAddressV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_EventListenerV2Id",
                table: "ListenerEventArchiveV2s",
                column: "EventListenerV2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventArchiveV2s_EventAddressV2s_EventAddressV2Id",
                table: "EventArchiveV2s",
                column: "EventAddressV2Id",
                principalTable: "EventAddressV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventArchiveV2s_EventParticipantV2s_EventParticipantV2Id",
                table: "EventArchiveV2s",
                column: "EventParticipantV2Id",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventListenerV2s_EventAddressV2s_EventAddressV2Id",
                table: "EventListenerV2s",
                column: "EventAddressV2Id",
                principalTable: "EventAddressV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventListenerV2s_EventParticipantV2s_EventParticipantV2Id",
                table: "EventListenerV2s",
                column: "EventParticipantV2Id",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventParticipantSecretV2s_EventParticipantV2s_EventParticipantV2Id",
                table: "EventParticipantSecretV2s",
                column: "EventParticipantV2Id",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventV2s_EventAddressV2s_EventAddressV2Id",
                table: "EventV2s",
                column: "EventAddressV2Id",
                principalTable: "EventAddressV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EventV2s_EventParticipantV2s_EventParticipantV2Id",
                table: "EventV2s",
                column: "EventParticipantV2Id",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventListenerV2s_EventListenerV2Id",
                table: "ListenerEventArchiveV2s",
                column: "EventListenerV2Id",
                principalTable: "EventListenerV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventParticipantV2s_EventParticipantV2Id",
                table: "ListenerEventArchiveV2s",
                column: "EventParticipantV2Id",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventAddressV2s_EventAddressV2Id",
                table: "ListenerEventV2s",
                column: "EventAddressV2Id",
                principalTable: "EventAddressV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventListenerV2s_EventListenerV2Id",
                table: "ListenerEventV2s",
                column: "EventListenerV2Id",
                principalTable: "EventListenerV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventParticipantV2s_EventParticipantV2Id",
                table: "ListenerEventV2s",
                column: "EventParticipantV2Id",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventV2s_EventV2Id",
                table: "ListenerEventV2s",
                column: "EventV2Id",
                principalTable: "EventV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventArchiveV2s_EventAddressV2s_EventAddressV2Id",
                table: "EventArchiveV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventArchiveV2s_EventParticipantV2s_EventParticipantV2Id",
                table: "EventArchiveV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventListenerV2s_EventAddressV2s_EventAddressV2Id",
                table: "EventListenerV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventListenerV2s_EventParticipantV2s_EventParticipantV2Id",
                table: "EventListenerV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventParticipantSecretV2s_EventParticipantV2s_EventParticipantV2Id",
                table: "EventParticipantSecretV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventV2s_EventAddressV2s_EventAddressV2Id",
                table: "EventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventV2s_EventParticipantV2s_EventParticipantV2Id",
                table: "EventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventListenerV2s_EventListenerV2Id",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventParticipantV2s_EventParticipantV2Id",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventAddressV2s_EventAddressV2Id",
                table: "ListenerEventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventListenerV2s_EventListenerV2Id",
                table: "ListenerEventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventParticipantV2s_EventParticipantV2Id",
                table: "ListenerEventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventV2s_EventV2Id",
                table: "ListenerEventV2s");

            migrationBuilder.DropIndex(
                name: "IX_ListenerEventArchiveV2s_EventListenerV2Id",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.RenameColumn(
                name: "EventV2Id",
                table: "ListenerEventV2s",
                newName: "EventId");

            migrationBuilder.RenameColumn(
                name: "EventParticipantV2Id",
                table: "ListenerEventV2s",
                newName: "ParticipantId");

            migrationBuilder.RenameColumn(
                name: "EventListenerV2Id",
                table: "ListenerEventV2s",
                newName: "EventListenerId");

            migrationBuilder.RenameColumn(
                name: "EventAddressV2Id",
                table: "ListenerEventV2s",
                newName: "EventAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_ListenerEventV2s_EventV2Id",
                table: "ListenerEventV2s",
                newName: "IX_ListenerEventV2s_EventId");

            migrationBuilder.RenameIndex(
                name: "IX_ListenerEventV2s_EventParticipantV2Id",
                table: "ListenerEventV2s",
                newName: "IX_ListenerEventV2s_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_ListenerEventV2s_EventListenerV2Id",
                table: "ListenerEventV2s",
                newName: "IX_ListenerEventV2s_EventListenerId");

            migrationBuilder.RenameIndex(
                name: "IX_ListenerEventV2s_EventAddressV2Id",
                table: "ListenerEventV2s",
                newName: "IX_ListenerEventV2s_EventAddressId");

            migrationBuilder.RenameColumn(
                name: "EventV2Id",
                table: "ListenerEventArchiveV2s",
                newName: "EventId");

            migrationBuilder.RenameColumn(
                name: "EventParticipantV2Id",
                table: "ListenerEventArchiveV2s",
                newName: "ParticipantId");

            migrationBuilder.RenameColumn(
                name: "EventListenerV2Id",
                table: "ListenerEventArchiveV2s",
                newName: "EventListenerId");

            migrationBuilder.RenameColumn(
                name: "EventAddressV2Id",
                table: "ListenerEventArchiveV2s",
                newName: "EventAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_ListenerEventArchiveV2s_EventParticipantV2Id",
                table: "ListenerEventArchiveV2s",
                newName: "IX_ListenerEventArchiveV2s_ParticipantId");

            migrationBuilder.RenameColumn(
                name: "EventParticipantV2Secret",
                table: "EventV2s",
                newName: "ParticipantSecret");

            migrationBuilder.RenameColumn(
                name: "EventParticipantV2Id",
                table: "EventV2s",
                newName: "ParticipantId");

            migrationBuilder.RenameColumn(
                name: "EventAddressV2Id",
                table: "EventV2s",
                newName: "EventAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_EventV2s_EventParticipantV2Id",
                table: "EventV2s",
                newName: "IX_EventV2s_ParticipantId");

            migrationBuilder.RenameColumn(
                name: "EventParticipantV2Id",
                table: "EventParticipantSecretV2s",
                newName: "ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_EventParticipantSecretV2s_EventParticipantV2Id",
                table: "EventParticipantSecretV2s",
                newName: "IX_EventParticipantSecretV2s_ParticipantId");

            migrationBuilder.RenameColumn(
                name: "EventParticipantV2Id",
                table: "EventListenerV2s",
                newName: "ParticipantId");

            migrationBuilder.RenameColumn(
                name: "EventAddressV2Id",
                table: "EventListenerV2s",
                newName: "EventAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_EventListenerV2s_EventParticipantV2Id",
                table: "EventListenerV2s",
                newName: "IX_EventListenerV2s_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_EventListenerV2s_EventAddressV2Id",
                table: "EventListenerV2s",
                newName: "IX_EventListenerV2s_EventAddressId");

            migrationBuilder.RenameColumn(
                name: "EventParticipantV2Secret",
                table: "EventArchiveV2s",
                newName: "ParticipantSecret");

            migrationBuilder.RenameColumn(
                name: "EventParticipantV2Id",
                table: "EventArchiveV2s",
                newName: "ParticipantId");

            migrationBuilder.RenameColumn(
                name: "EventAddressV2Id",
                table: "EventArchiveV2s",
                newName: "EventAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_EventArchiveV2s_EventParticipantV2Id",
                table: "EventArchiveV2s",
                newName: "IX_EventArchiveV2s_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_EventArchiveV2s_EventAddressV2Id",
                table: "EventArchiveV2s",
                newName: "IX_EventArchiveV2s_EventAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_EventListenerId",
                table: "ListenerEventArchiveV2s",
                column: "EventListenerId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventArchiveV2s_EventAddressV2s_EventAddressId",
                table: "EventArchiveV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_EventParticipantSecretV2s_EventParticipantV2s_ParticipantId",
                table: "EventParticipantSecretV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_ListenerEventArchiveV2s_EventListenerV2s_EventListenerId",
                table: "ListenerEventArchiveV2s",
                column: "EventListenerId",
                principalTable: "EventListenerV2s",
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
    }
}
