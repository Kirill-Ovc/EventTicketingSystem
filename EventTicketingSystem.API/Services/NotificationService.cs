using System.Globalization;
using EventTicketingSystem.API.Interfaces;
using EventTicketingSystem.API.Messaging;
using EventTicketingSystem.DataAccess.Interfaces;

namespace EventTicketingSystem.API.Services
{
    public class NotificationService : INotificationService
    {
        private const string CheckoutCompleted = "ticket successfully checked out";
        private const string CheckoutStart = "ticket added to checkout";
        private const string OrderSummaryTemplate = "Event: {0} Seat: {1} Price: {2}";
        private readonly IMessageSession _messageSession;
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookingSeatRepository _bookingSeatRepository;

        public NotificationService(IMessageSession messageSession,
            IBookingRepository bookingRepository,
            IUserRepository userRepository,
            IBookingSeatRepository bookingSeatRepository)
        {
            _messageSession = messageSession;
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _bookingSeatRepository = bookingSeatRepository;
        }

        public async Task SendNotificationTest(string operationName)
        {
            var sendNotification = new SendNotification
            {
                OperationName = "Test",
                CustomerName = "CustomerName",
                CustomerEmail = "CustomerEmail",
                OrderAmount = "OrderAmount",
                OrderSummary = new List<string>()
            };

            await _messageSession.Send(sendNotification);
        }

        public async Task NotifyCheckoutStartedAsync(int bookingId)
        {
            await SendNotification(CheckoutStart, bookingId);
        }

        public async Task NotifyCheckoutCompletedAsync(int bookingId)
        {
            await SendNotification(CheckoutCompleted, bookingId);
        }

        private async Task SendNotification(string operationName, int bookingId)
        {
            var booking = await _bookingRepository.Find(bookingId);
            var user = await _userRepository.Find(booking.UserId);
            var cartItems = await _bookingSeatRepository.GetSeats(bookingId);
            var orderSummary = cartItems
                .Select(ci => string.Format(OrderSummaryTemplate, ci.EventSeat.Event.Name, ci.EventSeat.Name, ci.Price))
                .ToList();

            var sendNotification = new SendNotification
            {
                OperationName = operationName,
                CustomerName = user.Name,
                CustomerEmail = user.Email,
                OrderAmount = booking.Price.ToString(CultureInfo.InvariantCulture),
                OrderSummary = orderSummary
            };

            await _messageSession.Send(sendNotification);
        }
    }
}
