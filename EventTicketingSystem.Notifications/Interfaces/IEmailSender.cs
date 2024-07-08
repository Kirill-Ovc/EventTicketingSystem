namespace EventTicketingSystem.Notifications.Interfaces
{
    internal interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
