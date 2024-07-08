namespace EventTicketingSystem.API.Interfaces
{
    public interface INotificationService
    {
        Task SendNotificationTest(string operationName);

        Task NotifyCheckoutStartedAsync(int bookingId);

        Task NotifyCheckoutCompletedAsync(int bookingId);
    }
}