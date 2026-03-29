using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProToolRent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SwapSchemeUserAndProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "Orders",
                newName: "UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "ProfileId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Orders",
                newName: "UserProfileId");
        }
    }
}
