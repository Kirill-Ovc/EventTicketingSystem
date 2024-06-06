namespace EventTicketingSystem.API.Exceptions
{
    public class BusinessException : InvalidOperationException
    {
        public BusinessException(string message) : base(message)
        {
        }

        public BusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
