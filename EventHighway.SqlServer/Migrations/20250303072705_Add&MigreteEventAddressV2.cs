// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventHighway.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddMigreteEventAddressV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EventAddressId",
                table: "EventV2s",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "EventAddressV2",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventAddressV2", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventV2s_EventAddressId",
                table: "EventV2s",
                column: "EventAddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventV2s_EventAddressV2_EventAddressId",
                table: "EventV2s",
                column: "EventAddressId",
                principalTable: "EventAddressV2",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventV2s_EventAddressV2_EventAddressId",
                table: "EventV2s");

            migrationBuilder.DropTable(
                name: "EventAddressV2");

            migrationBuilder.DropIndex(
                name: "IX_EventV2s_EventAddressId",
                table: "EventV2s");

            migrationBuilder.DropColumn(
                name: "EventAddressId",
                table: "EventV2s");
        }
    }
}
