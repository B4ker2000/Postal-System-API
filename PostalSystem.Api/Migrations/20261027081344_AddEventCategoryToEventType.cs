using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PostalSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddEventCategoryToEventType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Category",
                table: "EventTypes",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "EventTypes");
        }
    }
}
