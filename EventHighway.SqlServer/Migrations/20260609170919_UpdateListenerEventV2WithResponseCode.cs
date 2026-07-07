using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdateListenerEventV2WithResponseCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResponseReasonPhrase",
                table: "ListenerEventV2s",
                newName: "ResponseMessage");

            migrationBuilder.AddColumn<string>(
                name: "ResponseCode",
                table: "ListenerEventV2s",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseCode",
                table: "ListenerEventV2s");

            migrationBuilder.RenameColumn(
                name: "ResponseMessage",
                table: "ListenerEventV2s",
                newName: "ResponseReasonPhrase");
        }
    }
}
