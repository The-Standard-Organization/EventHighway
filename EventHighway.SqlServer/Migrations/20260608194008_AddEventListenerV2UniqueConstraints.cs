using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddEventListenerV2UniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HandlerConfigurations_EventListenerV2s_EventListenerId",
                table: "HandlerConfigurations");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EventListenerV2s",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EventAddressV2s",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventListenerV2s_Name",
                table: "EventListenerV2s",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EventAddressV2s_Name",
                table: "EventAddressV2s",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_HandlerConfigurations_EventListenerV2s_EventListenerId",
                table: "HandlerConfigurations",
                column: "EventListenerId",
                principalTable: "EventListenerV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HandlerConfigurations_EventListenerV2s_EventListenerId",
                table: "HandlerConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_EventListenerV2s_Name",
                table: "EventListenerV2s");

            migrationBuilder.DropIndex(
                name: "IX_EventAddressV2s_Name",
                table: "EventAddressV2s");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EventListenerV2s",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EventAddressV2s",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HandlerConfigurations_EventListenerV2s_EventListenerId",
                table: "HandlerConfigurations",
                column: "EventListenerId",
                principalTable: "EventListenerV2s",
                principalColumn: "Id");
        }
    }
}
