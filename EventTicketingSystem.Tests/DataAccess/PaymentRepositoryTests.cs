using EventTicketingSystem.DataAccess.Interfaces;
using EventTicketingSystem.DataAccess.Models.Context;
using EventTicketingSystem.DataAccess.Models.Entities;
using EventTicketingSystem.DataAccess.Models.Enums;
using EventTicketingSystem.DataAccess.Services;
using EventTicketingSystem.Tests.Helpers;
using EventTicketingSystem.Tests.Seed;
using FluentAssertions;

namespace EventTicketingSystem.Tests.DataAccess
{
    [TestFixture]
    [Parallelizable(ParallelScope.None)]
    public class PaymentRepositoryTests
    {
        private readonly TestDataReader _dataProvider;
        private IPaymentRepository _paymentRepository;
        private DatabaseContext _context;

        public PaymentRepositoryTests()
        {
            _dataProvider = new TestDataReader();
        }

        [SetUp]
        public async Task Setup()
        {
            _context = DatabaseHelper.CreateDbContext();
            _paymentRepository = new PaymentRepository(_context);
            var seeder = new TestDataSeeder(_context, _dataProvider);
            await seeder.SeedUsers();
            await seeder.SeedBookings();
            await seeder.SeedPayments();
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public async Task PaymentRepository_GetPaymentsByBooking_ReturnsPayments()
        {
            var expectedPayment = new Payment
            {
                Id = 2,
                BookingId = 2,
                Amount = 1000,
                PaymentStatus = PaymentStatus.Paid,
                PaymentDate = DateTime.Today,
                PaymentMethod = PaymentMethod.CreditCard

            };

            var payments = await _paymentRepository.GetPaymentsByBooking(expectedPayment.BookingId);

            payments.Should().ContainSingle();
            payments.First().Should().BeEquivalentTo(expectedPayment, options => 
                options.Excluding(o => o.Booking));
        }

        [Test]
        public async Task PaymentRepository_GetPaymentsByUser_ReturnsPayments()
        {
            var expectedPayments = new List<Payment>()
            {
                new Payment
                {
                    Id = 1,
                    BookingId = 1,
                    Amount = 1000,
                    PaymentStatus = PaymentStatus.Paid,
                    PaymentDate = DateTime.Today,
                    PaymentMethod = PaymentMethod.CreditCard
                },
                new Payment
                {
                    Id = 6,
                    BookingId = 6,
                    Amount = 2000,
                    PaymentStatus = PaymentStatus.Pending
                }
            };

            var payments = await _paymentRepository.GetPaymentsByUser(1);

            payments.Should().BeEquivalentTo(expectedPayments, options => 
                               options.Excluding(o => o.Booking));
        }

        [Test]
        public async Task PaymentRepository_GetPendingPayments_ReturnsPayments()
        {
            var expectedPayments = _dataProvider.GetPayments()
                .Where(p => p.PaymentStatus == PaymentStatus.Pending).ToList();

            var payments = await _paymentRepository.GetPendingPayments();

            payments.Should().BeEquivalentTo(expectedPayments, options => 
                                              options.Excluding(o => o.Booking));
        }
    }
}
