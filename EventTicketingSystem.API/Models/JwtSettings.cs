namespace EventTicketingSystem.API.Models
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int TokenExpirationHours { get; set; }
    }
}
