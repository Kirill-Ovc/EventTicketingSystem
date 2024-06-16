using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventTicketingSystem.DataAccess.Migrations
{
    public partial class AfterModule3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_EventSeats_EventSeatId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_EventSeatId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "EventSeatId",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "Token",
                table: "Bookings",
                newName: "Uuid");

            migrationBuilder.CreateTable(
                name: "BookingSeats",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookingId = table.Column<int>(type: "INTEGER", nullable: false),
                    EventSeatId = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    TicketLevel = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingSeats", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingSeats_EventSeats_EventSeatId",
                        column: x => x.EventSeatId,
                        principalTable: "EventSeats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingSeats_EventSeatId",
                table: "BookingSeats",
                column: "EventSeatId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingSeats");

            migrationBuilder.RenameColumn(
                name: "Uuid",
                table: "Bookings",
                newName: "Token");

            migrationBuilder.AddColumn<int>(
                name: "EventSeatId",
                table: "Bookings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_EventSeatId",
                table: "Bookings",
                column: "EventSeatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_EventSeats_EventSeatId",
                table: "Bookings",
                column: "EventSeatId",
                principalTable: "EventSeats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
