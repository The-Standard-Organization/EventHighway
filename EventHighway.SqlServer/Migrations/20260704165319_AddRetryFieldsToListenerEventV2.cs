using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddRetryFieldsToListenerEventV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DispatchedDate",
                table: "ListenerEventV2s",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "NextRetryAttemptNotBefore",
                table: "ListenerEventV2s",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemainingRetryAttempts",
                table: "ListenerEventV2s",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RetryAttemptsAllowed",
                table: "ListenerEventV2s",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DispatchedDate",
                table: "ListenerEventV2s");

            migrationBuilder.DropColumn(
                name: "NextRetryAttemptNotBefore",
                table: "ListenerEventV2s");

            migrationBuilder.DropColumn(
                name: "RemainingRetryAttempts",
                table: "ListenerEventV2s");

            migrationBuilder.DropColumn(
                name: "RetryAttemptsAllowed",
                table: "ListenerEventV2s");
        }
    }
}
