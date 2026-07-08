using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedListenerEventArchiveV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResponseReasonPhrase",
                table: "ListenerEventArchiveV2s",
                newName: "ResponseMessage");

            migrationBuilder.AddColumn<string>(
                name: "ResponseCode",
                table: "ListenerEventArchiveV2s",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseCode",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.RenameColumn(
                name: "ResponseMessage",
                table: "ListenerEventArchiveV2s",
                newName: "ResponseReasonPhrase");
        }
    }
}
