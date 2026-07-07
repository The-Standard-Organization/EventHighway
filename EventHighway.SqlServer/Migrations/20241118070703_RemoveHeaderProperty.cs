// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHeaderProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Header",
                table: "EventListeners");

            migrationBuilder.RenameColumn(
                name: "Secret",
                table: "EventListeners",
                newName: "HeaderSecret");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HeaderSecret",
                table: "EventListeners",
                newName: "Secret");

            migrationBuilder.AddColumn<string>(
                name: "Header",
                table: "EventListeners",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
