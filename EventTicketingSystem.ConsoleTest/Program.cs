using EventTicketingSystem.ConsoleTest;

var seedService = ServiceProvider.GetSeedService();
await seedService.SeedAllData();

var userRepository = ServiceProvider.GetUserRepository();
var result = await userRepository.GetUsers();

Console.WriteLine(result.First().Name);