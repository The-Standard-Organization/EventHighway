using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedHandlerConfigurationsWithEventListenerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HandlerConfigurations_EventListenerV2s_EventListenerV2Id",
                table: "HandlerConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_HandlerConfigurations_EventListenerV2Id",
                table: "HandlerConfigurations");

            migrationBuilder.DropColumn(
                name: "EventListenerV2Id",
                table: "HandlerConfigurations");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "HandlerConfigurations",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EventListenerId",
                table: "HandlerConfigurations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_HandlerConfigurations_EventListenerId_Name",
                table: "HandlerConfigurations",
                columns: new[] { "EventListenerId", "Name" },
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_HandlerConfigurations_EventListenerV2s_EventListenerId",
                table: "HandlerConfigurations",
                column: "EventListenerId",
                principalTable: "EventListenerV2s",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HandlerConfigurations_EventListenerV2s_EventListenerId",
                table: "HandlerConfigurations");

            migrationBuilder.DropIndex(
                name: "IX_HandlerConfigurations_EventListenerId_Name",
                table: "HandlerConfigurations");

            migrationBuilder.DropColumn(
                name: "EventListenerId",
                table: "HandlerConfigurations");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "HandlerConfigurations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "EventListenerV2Id",
                table: "HandlerConfigurations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HandlerConfigurations_EventListenerV2Id",
                table: "HandlerConfigurations",
                column: "EventListenerV2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HandlerConfigurations_EventListenerV2s_EventListenerV2Id",
                table: "HandlerConfigurations",
                column: "EventListenerV2Id",
                principalTable: "EventListenerV2s",
                principalColumn: "Id");
        }
    }
}
