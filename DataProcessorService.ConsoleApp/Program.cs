using DataProcessorService.ConsoleApp.Configuration;
using DataProcessorService.ConsoleApp.Interfaces;
using DataProcessorService.ConsoleApp.Repositories;
using DataProcessorService.ConsoleApp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

class Program
{
    static async Task Main(string[] args)
    {
        await Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config =>
            {
                config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.Configure<RabbitMqOptions>(context.Configuration.GetSection("RabbitMq"));
                             
                services.AddSingleton<IModuleRepository>(x =>
                {
                    var config = x.GetRequiredService<IConfiguration>();                   
                    var connectionString = config.GetConnectionString("Default");
                    var repo = new ModuleRepository(connectionString);
                    repo.InitializeAsync().GetAwaiter().GetResult();
                    return repo;
                });
                services.AddSingleton<IModuleMessageHandler, ModuleMessageHandler>();
                services.AddSingleton<RabbitMqConsumerService>();
              
                services.AddHostedService<DataProcessorBackgroundService>();
            
                services.AddLogging(cfg =>
                {
                    cfg.AddConsole();
                    cfg.SetMinimumLevel(LogLevel.Information);
                });
            })
            .RunConsoleAsync();       
    }
}
