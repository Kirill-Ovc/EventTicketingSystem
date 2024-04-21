using EventTicketingSystem.ConsoleTest;

var seedService = ServiceProvider.GetSeedService();
await seedService.SeedAllData();

var repository = ServiceProvider.GetUserRepository();
var result = await repository.GetUsers();

Console.WriteLine(result.First().Name);