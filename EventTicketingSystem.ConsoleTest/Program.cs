using EventTicketingSystem.ConsoleTest;

var repository = ServiceProvider.GetUserRepository();
var result = await repository.GetUsers();

Console.WriteLine(result.First().Name);