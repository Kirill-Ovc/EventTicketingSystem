using EventTicketingSystem.ConsoleTest;

var repository = ServiceProvider.GetRepository();
var result = await repository.GetUsers();

Console.WriteLine(result.First().Name);