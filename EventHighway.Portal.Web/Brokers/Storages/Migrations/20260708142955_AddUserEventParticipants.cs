using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Portal.Web.Brokers.Storages.Migrations
{
    /// <inheritdoc />
    public partial class AddUserEventParticipants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserEventParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEventParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserEventParticipants_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserEventParticipants_EventParticipantId",
                table: "UserEventParticipants",
                column: "EventParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEventParticipants_UserId_EventParticipantId",
                table: "UserEventParticipants",
                columns: new[] { "UserId", "EventParticipantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserEventParticipants");
        }
    }
}
