using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class AddEventHandlerV2s : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventHandlerV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventHandlerV2s", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventHandlerV2s_Name",
                table: "EventHandlerV2s",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventHandlerV2s");
        }
    }
}
