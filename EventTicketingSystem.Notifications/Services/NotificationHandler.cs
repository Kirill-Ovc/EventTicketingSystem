using System.Text;
using EventTicketingSystem.Contract.Models;
using EventTicketingSystem.Notifications.Interfaces;
using Microsoft.Extensions.Logging;

namespace EventTicketingSystem.Notifications.Services
{
    internal class NotificationHandler : IHandleMessages<SendNotification>
    {
        private readonly ILogger<NotificationHandler> _logger;
        private readonly IEmailSender _emailSender;

        public NotificationHandler(ILogger<NotificationHandler> logger,
            IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task Handle(SendNotification message, IMessageHandlerContext context)
        {
            _logger.LogInformation("Notification received: {NotificationId} - {OperationName}.", 
                message.NotificationId, message.OperationName);

            var subject = "Notification: " + message.OperationName;
            await _emailSender.SendEmailAsync(message.CustomerEmail, subject, CreateMessage(message));
        }

        private string CreateMessage(SendNotification info)
        {
            var message = new StringBuilder();
            message.Append($"<p>Dear client</p><p>Your order was updated - {info.OperationName}</p>");
            message.Append($"<p>Order summary: </p><ul>");
            foreach (var item in info.OrderSummary)
            {
                message.Append($"<li>{item}</li>");
            }
            message.Append("</ul>");
            message.Append($"<h3>Total amount: {info.OrderAmount}</h3>");
            return message.ToString();
        }
    }
}
