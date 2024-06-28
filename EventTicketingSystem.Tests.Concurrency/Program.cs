using EventTicketingSystem.Tests.Concurrency;

Console.WriteLine("Creating Test Web Application");
var test = new ConcurrencyTest();
Console.WriteLine("Seeding test data");
await test.SeedTestData();

Console.WriteLine("Running concurrency test for optimistic concurrency strategy.\n" +
                  "Perform 1000 requests to ensure only 1 successful request is returned.");
test.Run();
