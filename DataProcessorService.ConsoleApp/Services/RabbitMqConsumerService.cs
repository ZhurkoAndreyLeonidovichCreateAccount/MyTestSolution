using DataProcessorService.ConsoleApp.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace DataProcessorService.ConsoleApp.Services
{
    public class RabbitMqConsumerService : IAsyncDisposable
    {
        private readonly RabbitMqOptions _options;
        private readonly ILogger<RabbitMqConsumerService> _logger;
        private IConnection? _connection;
        private IChannel? _channel;

        public RabbitMqConsumerService(IOptions<RabbitMqOptions> options, ILogger<RabbitMqConsumerService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        private async Task EnsureConnectedAsync()
        {
            if (_connection != null && _channel != null)
                return;

            var factory = new ConnectionFactory
            {
                HostName = _options.HostName,
                Port = _options.Port,
                UserName = _options.UserName,
                Password = _options.Password
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: _options.QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false);

            _logger.LogInformation("Connected to RabbitMQ ({Host}:{Port}), listening on '{Queue}'.",
                _options.HostName, _options.Port, _options.QueueName);
        }

        public async Task ConsumeAsync(Func<string, Task> handleMessageAsync, CancellationToken token)
        {
            await EnsureConnectedAsync();

            var consumer = new AsyncEventingBasicConsumer(_channel!);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                await handleMessageAsync(message);
            };

            await _channel!.BasicConsumeAsync(queue: _options.QueueName, autoAck: true, consumer: consumer);

            _logger.LogInformation("Started consuming messages from RabbitMQ queue '{Queue}'.", _options.QueueName);
            await Task.Delay(Timeout.Infinite, token);
        }

        public async ValueTask DisposeAsync()
        {
            if (_channel != null) await _channel.DisposeAsync();
            if (_connection != null) await _connection.DisposeAsync();
        }
    }
}
