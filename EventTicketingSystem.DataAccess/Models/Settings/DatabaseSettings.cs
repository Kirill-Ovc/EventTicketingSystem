namespace EventTicketingSystem.DataAccess.Models.Settings
{
    /// <summary>
    /// Settings for Database
    /// </summary>
    public class DatabaseSettings
    {
        /// <summary>
        /// Name of section in application settings
        /// </summary>
        public const string SectionName = "DatabaseSettings";

        /// <summary>
        /// Connection string for Database. Contains Host,Port,Database,Username,Password.
        /// </summary>
        public string ConnectionString { get; set; }
    }
}
