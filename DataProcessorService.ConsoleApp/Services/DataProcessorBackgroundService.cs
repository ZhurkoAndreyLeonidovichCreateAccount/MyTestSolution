using DataProcessorService.ConsoleApp.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataProcessorService.ConsoleApp.Services
{
    public class DataProcessorBackgroundService : BackgroundService
    {
        private readonly ILogger<DataProcessorBackgroundService> _logger;
        private readonly RabbitMqConsumerService _consumer;
        private readonly IModuleMessageHandler _handler;

        public DataProcessorBackgroundService(
            ILogger<DataProcessorBackgroundService> logger,
            RabbitMqConsumerService consumer,
            IModuleMessageHandler handler)
        {
            _logger = logger;
            _consumer = consumer;
            _handler = handler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting DataProcessorBackgroundService...");
            var retryDelay = TimeSpan.FromSeconds(3);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _consumer.ConsumeAsync(_handler.HandleAsync, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in RabbitMQ consumer. Will retry in {Delay}s...", retryDelay.TotalSeconds);
                }
              
                await Task.Delay(retryDelay, stoppingToken);
            }
        }
    }
}
