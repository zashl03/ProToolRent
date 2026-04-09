using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProToolRent.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToTool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Tools",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Tools");
        }
    }
}
