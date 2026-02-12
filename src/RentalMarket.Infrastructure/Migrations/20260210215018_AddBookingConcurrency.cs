using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalMarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingConcurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TempFlag",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "ListingID",
                table: "Bookings",
                newName: "ListingId");

            migrationBuilder.RenameColumn(
                name: "ID",
                table: "Bookings",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_ListingID_StartDate_EndDate",
                table: "Bookings",
                newName: "IX_Bookings_ListingId_StartDate_EndDate");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Bookings",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "ListingId",
                table: "Bookings",
                newName: "ListingID");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Bookings",
                newName: "ID");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_ListingId_StartDate_EndDate",
                table: "Bookings",
                newName: "IX_Bookings_ListingID_StartDate_EndDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "TempFlag",
                table: "Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
