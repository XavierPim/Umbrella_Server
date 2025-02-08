using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Umbrella_Server.Migrations
{
    /// <inheritdoc />
    public partial class MoveAttendeeToMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Attendees");

            migrationBuilder.AddColumn<bool>(
                name: "CanCall",
                table: "Members",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CanMessage",
                table: "Members",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RsvpStatus",
                table: "Members",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanCall",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CanMessage",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "RsvpStatus",
                table: "Members");

            migrationBuilder.CreateTable(
                name: "Attendees",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CanCall = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CanMessage = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    RsvpStatus = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendees", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_Attendees_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });
        }
    }
}
