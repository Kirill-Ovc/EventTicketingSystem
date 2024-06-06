namespace EventTicketingSystem.API.Exceptions
{
    public class EntityNotFoundException : InvalidOperationException
    {
        public EntityNotFoundException(string message) : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
