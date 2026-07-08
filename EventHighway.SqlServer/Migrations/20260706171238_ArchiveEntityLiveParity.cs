using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class ArchiveEntityLiveParity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventArchiveV2s_EventAddressV2Id",
                table: "EventArchiveV2s");

            migrationBuilder.AddColumn<Guid>(
                name: "CorrelationId",
                table: "ListenerEventArchiveV2s",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DispatchedDate",
                table: "ListenerEventArchiveV2s",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "NextRetryAttemptNotBefore",
                table: "ListenerEventArchiveV2s",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemainingRetryAttempts",
                table: "ListenerEventArchiveV2s",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RetryAttemptsAllowed",
                table: "ListenerEventArchiveV2s",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ContentHash",
                table: "EventArchiveV2s",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_RetryClassification",
                table: "ListenerEventArchiveV2s",
                columns: new[] { "Status", "RemainingRetryAttempts" });

            migrationBuilder.CreateIndex(
                name: "IX_EventArchiveV2s_ContentHash",
                table: "EventArchiveV2s",
                columns: new[] { "EventAddressV2Id", "ContentHash" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ListenerEventArchiveV2s_RetryClassification",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropIndex(
                name: "IX_EventArchiveV2s_ContentHash",
                table: "EventArchiveV2s");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropColumn(
                name: "DispatchedDate",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropColumn(
                name: "NextRetryAttemptNotBefore",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropColumn(
                name: "RemainingRetryAttempts",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropColumn(
                name: "RetryAttemptsAllowed",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropColumn(
                name: "ContentHash",
                table: "EventArchiveV2s");

            migrationBuilder.CreateIndex(
                name: "IX_EventArchiveV2s_EventAddressV2Id",
                table: "EventArchiveV2s",
                column: "EventAddressV2Id");
        }
    }
}
