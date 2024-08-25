using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Reflection;

namespace EventTicketingSystem.API.Helpers
{
    public class VersionHealthCheck : IHealthCheck
    {
        private readonly string _version;

        public VersionHealthCheck()
        {
            _version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            // Return Healthy status with version information in data
            var data = new Dictionary<string, object>
            {
                { "version", _version },
            };
            return Task.FromResult(HealthCheckResult.Healthy(null, data));
        }
    }
}
