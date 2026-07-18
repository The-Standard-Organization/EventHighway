using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSecretRequiredToEventParticipantV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSecretRequired",
                table: "EventParticipantV2s",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSecretRequired",
                table: "EventParticipantV2s");
        }
    }
}
