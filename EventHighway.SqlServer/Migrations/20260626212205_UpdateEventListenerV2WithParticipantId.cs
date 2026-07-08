using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventListenerV2WithParticipantId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParticipantId",
                table: "EventListenerV2s",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventListenerV2s_ParticipantId",
                table: "EventListenerV2s",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventListenerV2s_EventParticipantV2s_ParticipantId",
                table: "EventListenerV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventListenerV2s_EventParticipantV2s_ParticipantId",
                table: "EventListenerV2s");

            migrationBuilder.DropIndex(
                name: "IX_EventListenerV2s_ParticipantId",
                table: "EventListenerV2s");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "EventListenerV2s");
        }
    }
}
