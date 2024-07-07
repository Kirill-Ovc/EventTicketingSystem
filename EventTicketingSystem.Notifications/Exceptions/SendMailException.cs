namespace EventTicketingSystem.Notifications.Exceptions
{
    public class SendMailException : Exception
    {
        public SendMailException(string message) : base(message)
        {
        }

        public SendMailException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
