using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddEventParticipantV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParticipantId",
                table: "ListenerEventV2s",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParticipantId",
                table: "ListenerEventArchiveV2s",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParticipantId",
                table: "EventV2s",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParticipantSecret",
                table: "EventV2s",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParticipantId",
                table: "EventArchiveV2s",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParticipantSecret",
                table: "EventArchiveV2s",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EventParticipantV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipantV2s", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventParticipantSecretV2s",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Secret = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ActiveFrom = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ActiveTo = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventParticipantSecretV2s", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventParticipantSecretV2s_EventParticipantV2s_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "EventParticipantV2s",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventV2s_ParticipantId",
                table: "ListenerEventV2s",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_ListenerEventArchiveV2s_ParticipantId",
                table: "ListenerEventArchiveV2s",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_EventV2s_ParticipantId",
                table: "EventV2s",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_EventArchiveV2s_ParticipantId",
                table: "EventArchiveV2s",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipantSecretV2s_Id_Secret",
                table: "EventParticipantSecretV2s",
                columns: new[] { "Id", "Secret" },
                unique: true,
                filter: "[Secret] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipantSecretV2s_ParticipantId",
                table: "EventParticipantSecretV2s",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventArchiveV2s_EventParticipantV2s_ParticipantId",
                table: "EventArchiveV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_EventV2s_EventParticipantV2s_ParticipantId",
                table: "EventV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventParticipantV2s_ParticipantId",
                table: "ListenerEventArchiveV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ListenerEventV2s_EventParticipantV2s_ParticipantId",
                table: "ListenerEventV2s",
                column: "ParticipantId",
                principalTable: "EventParticipantV2s",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventArchiveV2s_EventParticipantV2s_ParticipantId",
                table: "EventArchiveV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_EventV2s_EventParticipantV2s_ParticipantId",
                table: "EventV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventArchiveV2s_EventParticipantV2s_ParticipantId",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropForeignKey(
                name: "FK_ListenerEventV2s_EventParticipantV2s_ParticipantId",
                table: "ListenerEventV2s");

            migrationBuilder.DropTable(
                name: "EventParticipantSecretV2s");

            migrationBuilder.DropTable(
                name: "EventParticipantV2s");

            migrationBuilder.DropIndex(
                name: "IX_ListenerEventV2s_ParticipantId",
                table: "ListenerEventV2s");

            migrationBuilder.DropIndex(
                name: "IX_ListenerEventArchiveV2s_ParticipantId",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropIndex(
                name: "IX_EventV2s_ParticipantId",
                table: "EventV2s");

            migrationBuilder.DropIndex(
                name: "IX_EventArchiveV2s_ParticipantId",
                table: "EventArchiveV2s");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "ListenerEventV2s");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "ListenerEventArchiveV2s");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "EventV2s");

            migrationBuilder.DropColumn(
                name: "ParticipantSecret",
                table: "EventV2s");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "EventArchiveV2s");

            migrationBuilder.DropColumn(
                name: "ParticipantSecret",
                table: "EventArchiveV2s");
        }
    }
}
