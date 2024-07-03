namespace EventTicketingSystem.API.Messaging
{
    public class SendNotification : ICommand
    {
        public Guid NotificationId { get; set; } = Guid.NewGuid();

        public string OperationName { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string CustomerEmail { get; set; }

        public string CustomerName { get; set; }

        public IList<string> OrderSummary { get; set; }

        public string OrderAmount { get; set; }
    }
}
