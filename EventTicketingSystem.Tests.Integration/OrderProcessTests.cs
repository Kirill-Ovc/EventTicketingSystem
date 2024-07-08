using System.Net;
using System.Text;
using System.Text.Json;
using EventTicketingSystem.API.Models;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using EventTicketingSystem.Tests.Integration.Seed;
using FluentAssertions;

namespace EventTicketingSystem.Tests.Integration
{
    public class OrderProcessTests
    {
        private readonly HttpClient _client;
        private readonly TestDataSeeder _seeder;

        private const int CityId = 10; // San Francisco
        private const int VenueId = 4;
        private const int EventId = 31;
        private const string EventName = "Macworld Conference";
        private const string GetEventsByCityUrl = "api/events/{cityId}";
        private const string GetSectionsUrl = "api/venues/{venueId}/sections";
        private const string GetSeatsUrl = "api/events/{eventId}/sections/{sectionId}/seats";
        private const string AddToCartUrl = "api/orders/carts/{cartId}";
        private const string RemoveFromCartUrl = "api/orders/carts/{cartId}/events/{eventId}/seats/{seatId}";
        private const string BookCartUrl = "api/orders/carts/{cartId}/book";
        private const string PaymentUrl = "api/payments/{paymentId}/complete";

        public OrderProcessTests()
        {
            var app = new TestWebApplication();
            app.AuthenticateAsync().Wait();
            _client = app.Client;
            var dataProvider = new TestDataProvider();
            _seeder = new TestDataSeeder(app.DbContext, dataProvider);
        }

        [SetUp]
        public async Task Setup()
        {
            await _seeder.SeedVenues();
            await _seeder.SeedSeats(VenueId);
            await _seeder.SeedEvents();
            await _seeder.SeedEventSeats(VenueId, EventId);
            await _seeder.SeedOffers(VenueId, EventId);
        }

        [Test]
        public async Task GetEvents()
        {
            // Arrange
            var url = GetEventsByCityUrl.Replace("{cityId}", CityId.ToString());

            // Act
            var response = _client.GetAsync(url).Result;

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.DeserializeAsync<List<EventDto>>();
            result.Count.Should().Be(1);
            result[0].Should().BeEquivalentTo(_testEvent);
        }

        [Test]
        public async Task GetVenueSections()
        {
            // Arrange
            var url = GetSectionsUrl.Replace("{venueId}", VenueId.ToString());

            // Act
            var response = _client.GetAsync(url).Result;

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.DeserializeAsync<List<Section>>();
            result.Should().BeEquivalentTo(_testSections);
        }

        [Test]
        public async Task GetSeats()
        {
            // Arrange
            var url = GetSeatsUrl.Replace("{eventId}", EventId.ToString()).Replace("{sectionId}", "1");

            // Act
            var response = _client.GetAsync(url).Result;

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.DeserializeAsync<List<EventSeatDto>>();
            result.Count.Should().Be(400);
            var firstSeat = result.OrderBy(s => s.Id).First();
            firstSeat.Should().BeEquivalentTo(_testSeat);
        }

        [Test]
        public async Task OrderSeat()
        {
            var cart = await AddToCart();
            var paymentId = await BookCart(cart.CartId);
            await CompletePayment(paymentId);

            // Assert manually - 2 notifications in the message queue
        }

        private async Task<Cart> AddToCart()
        {
            // Arrange
            var cartId = "cart-test-12345";
            var url = AddToCartUrl.Replace("{cartId}", cartId);
            var order = new SeatOrder
            {
                UserId = 2,
                EventId = EventId,
                EventSeatId = 1,
                OfferId = 1
            };

            // Act
            var postContent = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(url, postContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.DeserializeAsync<Cart>();
            result.ExpirationTimeStamp.Should().BeAfter(DateTime.UtcNow);
            _testCart.ExpirationTimeStamp = result.ExpirationTimeStamp;
            result.Should().BeEquivalentTo(_testCart);

            return result;
        }

        private async Task<int> BookCart(string cartId)
        {
            // Arrange
            var url = BookCartUrl.Replace("{cartId}", cartId);

            // Act
            var response = await _client.PutAsync(url, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var paymentId = await response.Content.DeserializeAsync<int>();
            paymentId.Should().BeGreaterThan(0);

            return paymentId;
        }

        private async Task CompletePayment(int paymentId)
        {
            // Arrange
            var url = PaymentUrl.Replace("{paymentId}", paymentId.ToString());

            // Act
            var response = await _client.PostAsync(url, null);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Test]
        public async Task OrderSeatAndRelease()
        {
            await AddToCart2Seats();
            var cart = await RemoveFromCart();
            var paymentId = await BookCart(cart.CartId);
            await CompletePayment(paymentId);
        }

        private async Task<Cart> AddToCart2Seats()
        {
            // Arrange
            var cartId = _testCartAfterSeatRemoved.CartId;
            var url = AddToCartUrl.Replace("{cartId}", cartId);
            var order1 = new SeatOrder
            {
                UserId = 2,
                EventId = EventId,
                EventSeatId = 2,
                OfferId = 1
            };
            var order2 = new SeatOrder
            {
                UserId = 2,
                EventId = EventId,
                EventSeatId = 3,
                OfferId = 1
            };

            // Act
            var postContent = new StringContent(JsonSerializer.Serialize(order1), Encoding.UTF8, "application/json");
            _ = await _client.PostAsync(url, postContent);
            postContent = new StringContent(JsonSerializer.Serialize(order2), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(url, postContent);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.DeserializeAsync<Cart>();
            result.ExpirationTimeStamp.Should().BeAfter(DateTime.UtcNow);
            _testCart2Seats.ExpirationTimeStamp = result.ExpirationTimeStamp;
            result.Should().BeEquivalentTo(_testCart2Seats);

            return result;
        }

        private async Task<Cart> RemoveFromCart()
        {
            // Arrange
            var cartId = _testCartAfterSeatRemoved.CartId;
            var url = RemoveFromCartUrl
                .Replace("{cartId}", cartId)
                .Replace("{eventId}", EventId.ToString())
                .Replace("{seatId}", "2");

            // Act
            var response = await _client.DeleteAsync(url);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.DeserializeAsync<Cart>();
            result.ExpirationTimeStamp.Should().BeAfter(DateTime.UtcNow);
            _testCartAfterSeatRemoved.ExpirationTimeStamp = result.ExpirationTimeStamp;
            result.Should().BeEquivalentTo(_testCartAfterSeatRemoved);

            return result;
        }


        #region TestData

        private readonly EventDto _testEvent = new EventDto()
        {
            Id = EventId,
            VenueId = VenueId,
            EventInfoId = 3,
            DataAndTime = new DateTime(2024, 09, 05, 12, 0, 0, DateTimeKind.Utc),
            Name = EventName,
            Type = "Conference",
            VenueName = "Moscone Center"
        };

        private readonly List<Section> _testSections = new List<Section>()
        {
            new Section
            {
                Id = 1,
                VenueId = VenueId,
                Name = "Section A",
                Capacity = 400
            },
            new Section
            {
                Id = 2,
                VenueId = VenueId,
                Name = "Section B",
                Capacity = 100
            },
            new Section
            {
                Id = 3,
                VenueId = VenueId,
                Name = "Section C",
                Capacity = 100
            }
        };

        private readonly EventSeatDto _testSeat = new EventSeatDto
        {
            Id = 1,
            Name = "A1-1",
            EventId = EventId,
            SeatId = 1,
            SectionId = 1,
            RowNumber = 1,
            Status = EventSeatStatus.Available,
            Prices = new List<SeatOfferDto>()
            {
                new SeatOfferDto()
                {
                    OfferId = 1,
                    Price = 100,
                    TicketLevel = TicketLevel.Other
                }
            }
        };

        private readonly Cart _testCart = new Cart
        {
            CartId = "cart-test-12345",
            Status = "Active",
            UserId = 2,
            CartItems = new List<CartItem>
            {
                new CartItem
                {
                    EventId = EventId,
                    EventSeatId = 1,
                    Price = 100,
                    EventName = EventName
                }
            }
        };

        private readonly Cart _testCart2Seats = new Cart
        {
            CartId = "cart-test-2222",
            Status = "Active",
            UserId = 2,
            CartItems = new List<CartItem>
            {
                new CartItem
                {
                    EventId = EventId,
                    EventSeatId = 2,
                    Price = 100,
                    EventName = EventName
                },
                new CartItem
                {
                    EventId = EventId,
                    EventSeatId = 3,
                    Price = 100,
                    EventName = EventName
                }
            }
        };

        private readonly Cart _testCartAfterSeatRemoved = new Cart
        {
            CartId = "cart-test-2222",
            Status = "Active",
            UserId = 2,
            CartItems = new List<CartItem>
            {
                new CartItem
                {
                    EventId = EventId,
                    EventSeatId = 3,
                    Price = 100,
                    EventName = EventName
                }
            }
        };

        #endregion
    }
}
