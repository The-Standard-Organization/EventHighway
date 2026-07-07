using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddEventAddressV2AndEventListenerV2NavigationConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_EventListenerId",
                table: "ListenerEventArchiveV2s",
                column: "EventListenerId");

            migrationBuilder.CreateIndex(
                name: "IX_EventArchiveV2s_EventAddressId",
                table: "EventArchiveV2s",
                column: "EventAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventArchiveV2s_EventAddressV2s_EventAddressId",
                table: "EventArchiveV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventListenerV2s_EventListenerId",
                table: "ListenerEventArchiveV2s",
                column: "EventListenerId",
                principalTable: "EventListenerV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventArchiveV2s_EventAddressV2s_EventAddressId",
                table: "EventArchiveV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventListenerV2s_EventListenerId",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropIndex(
                name: "IX_ListenerEventArchiveV2s_EventListenerId",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropIndex(
                name: "IX_EventArchiveV2s_EventAddressId",
                table: "EventArchiveV2s");
        }
    }
}
