using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Movie_Management_API.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToShowTimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Bookings__UserID__5BE2A6F2",
                table: "Bookings");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ShowTimes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK__Bookings__UserID__5BE2A6F2",
                table: "Bookings",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Bookings__UserID__5BE2A6F2",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ShowTimes");

            migrationBuilder.AddForeignKey(
                name: "FK__Bookings__UserID__5BE2A6F2",
                table: "Bookings",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID");
        }
    }
}
