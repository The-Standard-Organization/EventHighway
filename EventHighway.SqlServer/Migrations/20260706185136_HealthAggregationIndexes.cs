using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class HealthAggregationIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ListenerEventV2s_EventAddressV2Id",
                table: "ListenerEventV2s");

            migrationBuilder.DropIndex(
                name: "IX_ListenerEventV2s_EventV2Id",
                table: "ListenerEventV2s");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_AddressCreatedDate",
                table: "ListenerEventV2s",
                columns: new[] { "EventAddressV2Id", "CreatedDate" })
                .Annotation("SqlServer:Include", new[] { "Status", "RemainingRetryAttempts" });

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_CreatedDate",
                table: "ListenerEventV2s",
                column: "CreatedDate")
                .Annotation("SqlServer:Include", new[] { "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_RetryClassification",
                table: "ListenerEventV2s",
                columns: new[] { "Status", "RemainingRetryAttempts" });

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_ToBeArchived",
                table: "ListenerEventV2s",
                column: "EventV2Id")
                .Annotation("SqlServer:Include", new[] { "Status", "RemainingRetryAttempts", "DispatchedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_AddressArchivedDate",
                table: "ListenerEventArchiveV2s",
                columns: new[] { "EventAddressV2Id", "ArchivedDate" })
                .Annotation("SqlServer:Include", new[] { "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_ArchivedDate",
                table: "ListenerEventArchiveV2s",
                column: "ArchivedDate")
                .Annotation("SqlServer:Include", new[] { "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_Status",
                table: "ListenerEventArchiveV2s",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_EventV2s_AddressCreatedDate",
                table: "EventV2s",
                columns: new[] { "EventAddressV2Id", "CreatedDate" })
                .Annotation("SqlServer:Include", new[] { "Status", "Type", "EventParticipantV2Id" });

            migrationBuilder.CreateIndex(
                name: "IX_EventV2s_CreatedDate",
                table: "EventV2s",
                column: "CreatedDate")
                .Annotation("SqlServer:Include", new[] { "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_EventV2s_StatusType",
                table: "EventV2s",
                columns: new[] { "Status", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_EventArchiveV2s_AddressArchivedDate",
                table: "EventArchiveV2s",
                columns: new[] { "EventAddressV2Id", "ArchivedDate" })
                .Annotation("SqlServer:Include", new[] { "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_EventArchiveV2s_ArchivedDate",
                table: "EventArchiveV2s",
                column: "ArchivedDate");

            migrationBuilder.CreateIndex(
                name: "IX_EventArchiveV2s_Status",
                table: "EventArchiveV2s",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ListenerEventV2s_AddressCreatedDate",
                table: "ListenerEventV2s");

            migrationBuilder.DropIndex(
                name: "IX_ListenerEventV2s_CreatedDate",
                table: "ListenerEventV2s");

            migrationBuilder.DropIndex(
                name: "IX_ListenerEventV2s_RetryClassification",
                table: "ListenerEventV2s");

            migrationBuilder.DropIndex(
                name: "IX_ListenerEventV2s_ToBeArchived",
                table: "ListenerEventV2s");

            migrationBuilder.DropIndex(
                name: "IX_ListenerEventArchiveV2s_AddressArchivedDate",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropIndex(
                name: "IX_ListenerEventArchiveV2s_ArchivedDate",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropIndex(
                name: "IX_ListenerEventArchiveV2s_Status",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropIndex(
                name: "IX_EventV2s_AddressCreatedDate",
                table: "EventV2s");

            migrationBuilder.DropIndex(
                name: "IX_EventV2s_CreatedDate",
                table: "EventV2s");

            migrationBuilder.DropIndex(
                name: "IX_EventV2s_StatusType",
                table: "EventV2s");

            migrationBuilder.DropIndex(
                name: "IX_EventArchiveV2s_AddressArchivedDate",
                table: "EventArchiveV2s");

            migrationBuilder.DropIndex(
                name: "IX_EventArchiveV2s_ArchivedDate",
                table: "EventArchiveV2s");

            migrationBuilder.DropIndex(
                name: "IX_EventArchiveV2s_Status",
                table: "EventArchiveV2s");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_EventAddressV2Id",
                table: "ListenerEventV2s",
                column: "EventAddressV2Id");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_EventV2Id",
                table: "ListenerEventV2s",
                column: "EventV2Id");
        }
    }
}
