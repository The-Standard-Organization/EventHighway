using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceSecretUniqueIndexWithParticipantSecret : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventParticipantSecretV2s_EventParticipantV2Id",
                table: "EventParticipantSecretV2s");

            migrationBuilder.DropIndex(
                name: "IX_EventParticipantSecretV2s_Id_Secret",
                table: "EventParticipantSecretV2s");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipantSecretV2s_EventParticipantV2Id_Secret",
                table: "EventParticipantSecretV2s",
                columns: new[] { "EventParticipantV2Id", "Secret" },
                unique: true,
                filter: "[Secret] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventParticipantSecretV2s_EventParticipantV2Id_Secret",
                table: "EventParticipantSecretV2s");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipantSecretV2s_EventParticipantV2Id",
                table: "EventParticipantSecretV2s",
                column: "EventParticipantV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipantSecretV2s_Id_Secret",
                table: "EventParticipantSecretV2s",
                columns: new[] { "Id", "Secret" },
                unique: true,
                filter: "[Secret] IS NOT NULL");
        }
    }
}
