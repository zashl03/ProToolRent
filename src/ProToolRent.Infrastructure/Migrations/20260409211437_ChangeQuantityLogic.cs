using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProToolRent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeQuantityLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReservedQuantity",
                table: "Tools",
                newName: "AvailableQuantity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AvailableQuantity",
                table: "Tools",
                newName: "ReservedQuantity");
        }
    }
}
