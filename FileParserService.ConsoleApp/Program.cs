using FileParserService.Application.Interfaces;
using FileParserService.Infrastructure.Configuration;
using FileParserService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

class Program
{
    static async Task Main(string[] args)
    {
        
        await Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {              
                services.AddSingleton<IFileParser, XmlFileParser>();
                services.AddHostedService<ModulePublisherService>();
                services.Configure<RabbitMqOptions>(context.Configuration.GetSection("RabbitMq"));
                services.Configure<FilePathOptions>(context.Configuration.GetSection("Files"));
            })
            .RunConsoleAsync();
    }
}
