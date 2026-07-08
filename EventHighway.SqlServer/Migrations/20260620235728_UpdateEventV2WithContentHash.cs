using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventV2WithContentHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventV2s_EventAddressId",
                table: "EventV2s");

            migrationBuilder.AlterColumn<string>(
                name: "EventName",
                table: "EventV2s",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentHash",
                table: "EventV2s",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventV2s_LoopDetection",
                table: "EventV2s",
                columns: new[] { "EventAddressId", "EventName", "ContentHash", "CreatedDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventV2s_LoopDetection",
                table: "EventV2s");

            migrationBuilder.DropColumn(
                name: "ContentHash",
                table: "EventV2s");

            migrationBuilder.AlterColumn<string>(
                name: "EventName",
                table: "EventV2s",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EventV2s_EventAddressId",
                table: "EventV2s",
                column: "EventAddressId");
        }
    }
}
