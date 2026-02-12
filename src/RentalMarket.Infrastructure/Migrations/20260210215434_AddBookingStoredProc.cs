using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentalMarket.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingStoredProc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE Sp_CreateBooking
                    @Id UNIQUEIDENTIFIER,
                    @ListingId UNIQUEIDENTIFIER,
                    @GuestId NVARCHAR(450),
                    @StartDate DATETIME2,
                    @EndDate DATETIME2,
                    @TotalPrice DECIMAL(18, 2),
                    @RowVersion ROWVERSION OUTPUT
                AS
                BEGIN
                set NOCOUNT ON;
                set transaction isolation level serializable;
                BEGIN TRANSACTION; -- <--- ADDED THIS

                if exists (
                    SELECT 1
                    FROM Bookings WITH (UPDLOCK, HOLDLOCK)
                    WHERE ListingId = @ListingId
                    And StartDate < @EndDate
                    And EndDate > @StartDate)
                BEGIN
                    Rollback Transaction;
                    Throw 50001, 'The listing is already booked for the selected dates.', 1;
                END

                INSERT INTO Bookings (Id, ListingId, GuestId, StartDate, EndDate, TotalPrice)
                VALUES (@Id, @ListingId, @GuestId, @StartDate, @EndDate, @TotalPrice);

                commit transaction;
                end;
            ");
        }

        // <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
