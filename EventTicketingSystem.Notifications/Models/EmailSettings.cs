namespace EventTicketingSystem.Notifications.Models
{
    internal class EmailSettings
    {
        public const string SectionName = "EmailSettings";

        public string ApiKey { get; set; }

        public string ApiSecret { get; set; }

        public string TestEmail { get; set; }

        public string SenderEmail { get; set; }
    }
}
