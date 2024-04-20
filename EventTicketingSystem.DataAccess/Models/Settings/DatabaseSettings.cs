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

        /// <summary>
        /// Connection string template for Database without a password. Contains Host,Port,Database,Username.
        /// Use '{0}' placeholder for password.
        /// </summary>
        public string ConnectionStringTemplate { get; set; }

        /// <summary>
        /// Password for Username in Connection string for Database.
        /// </summary>
        public string ConnectionStringPassword { get; set; }

        /// <summary>
        /// Method for getting connection string
        /// </summary>
        /// <returns>Connection string</returns>
        public string GetConnectionString()
        {
            if (!string.IsNullOrEmpty(ConnectionStringTemplate))
            {
                return string.Format(ConnectionStringTemplate, ConnectionStringPassword);
            }

            return ConnectionString;
        }
    }
}
