using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SmartRoom.API.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoomData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Building", "Capacity", "IsAvailable", "RoomName", "Type" },
                values: new object[,]
                {
                    { 1, "TowerA", 30, true, "Classroom 101", "Classroom" },
                    { 2, "TowerB", 20, true, "Laboratory 202", "Laboratory" },
                    { 3, "TowerC", 15, true, "Meeting Room 303", "MeetingRoom" },
                    { 4, "TowerA", 100, true, "Auditorium 404", "Auditorium" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Rooms",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
