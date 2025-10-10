using FileParserService.Application.Interfaces;
using FileParserService.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Exceptions;

namespace FileParserService.Infrastructure.Services
{
    public class ModulePublisherService : BackgroundService
    {
        private readonly IFileParser _parser;
        private readonly IConfiguration _configuration;
        private IModulePublisher? _publisher;
        private readonly string _folderPath;
        private readonly string _folderProcessedPath;
        private readonly ILogger<ModulePublisherService> _logger;

        public ModulePublisherService(IFileParser parser, IConfiguration configuration, ILogger<ModulePublisherService> logger, IOptions<FilePathOptions> fileOptions)
        {
            _parser = parser;
            _configuration = configuration;           
            _folderPath = Path.Combine(AppContext.BaseDirectory, fileOptions.Value.FolderPath);
            _folderProcessedPath = Path.Combine(AppContext.BaseDirectory, fileOptions.Value.FolderProcessedPath);
            Directory.CreateDirectory(_folderPath);
            Directory.CreateDirectory(_folderProcessedPath);
            _logger = logger;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await InitializeRabbitMqAsync();
            _logger.LogInformation("ModulePublisherService started.");
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var modules = await _parser.ParseAsync(_folderPath, stoppingToken);

                    if (!modules.Any())
                    {
                        _logger.LogInformation("No modules found — skipping publish.");
                    }
                    else if(_publisher == null)
                    {
                        _logger.LogWarning("Skipping message publish: RabbitMQ is not connected.");
                        await InitializeRabbitMqAsync();
                    }
                    else
                    {
                        await _publisher.PublishAsync(modules);
                        _logger.LogInformation("Published {Count} modules to RabbitMQ.", modules.Count());

                        foreach (var xmlFile in Directory.GetFiles(_folderPath, "*.xml"))
                        {
                            var destination = Path.Combine(_folderProcessedPath, Path.GetFileName(xmlFile));
                            File.Move(xmlFile, destination, overwrite: true);
                            _logger.LogInformation("Moved {Source} to {Destination}", xmlFile, destination);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while processing modules.");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_publisher is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();

            await base.StopAsync(cancellationToken);
        }

        private async Task InitializeRabbitMqAsync()
        {
            var rabbitOptions = _configuration.GetSection("RabbitMq").Get<RabbitMqOptions>();
            try
            {
                _publisher = await RabbitMqModulePublisher.CreateAsync(
                rabbitOptions.HostName,
                rabbitOptions.Port,
                rabbitOptions.UserName,
                rabbitOptions.Password,
                rabbitOptions.QueueName
                );
            }
            catch (BrokerUnreachableException ex)
            {
                _logger.LogError(ex,
                    "Failed to connect to RabbitMQ at {Host}:{Port}. Please check broker availability and credentials.",
                    rabbitOptions.HostName, rabbitOptions.Port);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error occurred while initializing RabbitMQ publisher at {Host}:{Port}.",
                    rabbitOptions.HostName, rabbitOptions.Port);
            }
        }
    }
}
