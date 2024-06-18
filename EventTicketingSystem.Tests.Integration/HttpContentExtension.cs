using System.Text.Json;

namespace EventTicketingSystem.Tests.Integration
{
    internal static class HttpContentExtension
    {
        public static async Task<T> DeserializeAsync<T>(this HttpContent content)
        {
            var data = await content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<T>(data, options);

            return result;
        }
    }
}
