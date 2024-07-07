using EventTicketingSystem.Notifications.Extensions;
using EventTicketingSystem.Notifications.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Notifications Consumer started.");

var configurationBuilder = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.Development.json", optional: true)
    .AddEnvironmentVariables();

IConfiguration configuration = configurationBuilder.Build();

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddConfiguration(configuration);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddLogging();
        services.AddServices();
        services.Configure<EmailSettings>(hostContext.Configuration.GetSection(EmailSettings.SectionName));
    });

builder.AddMessaging(configuration);

var host = builder.Build();
await host.RunAsync();
