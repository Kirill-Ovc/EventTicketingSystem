using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using EventTicketingSystem.DataAccess.Models.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EventTicketingSystem.Tests.Integration
{
    internal class TestWebApplication
    {
        public HttpClient Client { get; }

        public DatabaseContext DbContext { get; }

        public TestWebApplication()
        {
            var appFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.RemoveAll<DatabaseContext>();
                        services.RemoveAll<DbContextOptions<DatabaseContext>>();
                        services.AddDbContext<DatabaseContext>(options => options.UseInMemoryDatabase("TestDb"));
                    });
                });

            Client = appFactory.CreateClient();

            var scope = appFactory.Services.CreateScope();
            DbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
        }

        public async Task AuthenticateAsync()
        {
            var token = await GetTokenAsync();
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private async Task<string> GetTokenAsync()
        {
            var response = await Client.PostAsJsonAsync("api/identity/token", new { username = "test", password = "test" });
            var token = await response.Content.ReadAsStringAsync();
            return token;
        }
    }
}
