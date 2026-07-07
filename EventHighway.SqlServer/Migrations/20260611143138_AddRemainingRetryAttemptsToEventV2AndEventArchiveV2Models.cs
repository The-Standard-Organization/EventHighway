using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddRemainingRetryAttemptsToEventV2AndEventArchiveV2Models : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemainingRetryAttempts",
                table: "EventV2s",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RemainingRetryAttempts",
                table: "EventArchiveV2s",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainingRetryAttempts",
                table: "EventV2s");

            migrationBuilder.DropColumn(
                name: "RemainingRetryAttempts",
                table: "EventArchiveV2s");
        }
    }
}
