using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class DropEventParticipantV2SecretFromEventV2AndEventArchiveV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventParticipantV2Secret",
                table: "EventV2s");

            migrationBuilder.DropColumn(
                name: "EventParticipantV2Secret",
                table: "EventArchiveV2s");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventParticipantV2Secret",
                table: "EventV2s",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventParticipantV2Secret",
                table: "EventArchiveV2s",
                type: "text",
                nullable: true);
        }
    }
}
