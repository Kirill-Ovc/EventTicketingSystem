using System.Text;
using System.Text.Json;
using EventTicketingSystem.API.Models;
using EventTicketingSystem.Tests.Integration.Seed;
using NBomber.CSharp;

namespace EventTicketingSystem.Tests.Concurrency
{
    public class ConcurrencyTest
    {
        private readonly HttpClient _client;
        private readonly TestDataSeeder _seeder;

        private const int VenueId = 4;
        private const int EventId = 31;
        private const string AddToCartUrl = "api/orders/carts/{cartId}";

        public ConcurrencyTest()
        {
            var app = new TestWebApplication();
            app.AuthenticateAsync().Wait();
            _client = app.Client;
            var dataProvider = new TestDataProvider();
            _seeder = new TestDataSeeder(app.DbContext, dataProvider);
        }

        public async Task SeedTestData()
        {
            await _seeder.SeedVenues();
            await _seeder.SeedSeats(VenueId);
            await _seeder.SeedEvents();
            await _seeder.SeedEventSeats(VenueId, EventId);
            await _seeder.SeedOffers(VenueId, EventId);
        }

        public void Run()
        {
            var cartId = "cart-test-12345";
            var url = AddToCartUrl.Replace("{cartId}", cartId);

            WarmUpRequest(url, 1).Wait();
            WarmUpRequest(url, 2).Wait();
            WarmUpRequest(url, 3).Wait();

            var order = new SeatOrder
            {
                UserId = 2,
                EventId = EventId,
                EventSeatId = 5,
                OfferId = 1
            };
            var postContent = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");

            var scenario = Scenario.Create("AddToCart", async context =>
                {
                    var response = await _client.PostAsync(url, postContent);

                    return response.IsSuccessStatusCode
                        ? Response.Ok(statusCode: response.StatusCode.ToString())
                        : Response.Fail(statusCode: response.StatusCode.ToString());
                })
                .WithoutWarmUp()
                .WithLoadSimulations(Simulation.IterationsForConstant(1000, 2000));

            NBomberRunner
                .RegisterScenarios(scenario)
                .Run();
        }

        private async Task WarmUpRequest(string url, int seatId)
        {
            var order = new SeatOrder
            {
                UserId = 2,
                EventId = EventId,
                EventSeatId = seatId,
                OfferId = 1
            };
            var postContent = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(url, postContent);
            response.EnsureSuccessStatusCode();
        }
    }
}
