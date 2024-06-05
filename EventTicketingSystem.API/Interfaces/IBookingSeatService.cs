namespace EventTicketingSystem.API.Interfaces
{
    public interface IBookingSeatService
    {
        Task AddSeat(int bookingId, int eventSeatId, int offerId);
        Task RemoveSeat(int bookingId, int eventSeatId);
        Task BookSeats(int bookingId);
    }
}
