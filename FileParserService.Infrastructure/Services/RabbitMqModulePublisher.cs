using FileParserService.Application.Interfaces;
using FileParserService.Domain;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace FileParserService.Infrastructure.Services
{
    public class RabbitMqModulePublisher : IModulePublisher, IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _queueName;     

        private RabbitMqModulePublisher(IConnection connection, IChannel channel, string queueName)
        {
            _connection = connection;
            _channel = channel;
            _queueName = queueName;       
        }

        public static async Task<RabbitMqModulePublisher> CreateAsync(string hostName, int port, string userName, string password, string queueName)
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName,
                Port = port,
                UserName = userName,
                Password = password
            };
           
            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();
                    
            await channel.QueueDeclareAsync(queue: queueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
           
            return new RabbitMqModulePublisher(connection, channel, queueName);
        }

        public async Task PublishAsync(IEnumerable<Module> modules)
        {          
                var json = JsonSerializer.Serialize(modules);
                var body = Encoding.UTF8.GetBytes(json);

                await _channel.BasicPublishAsync(exchange: "",
                                      routingKey: _queueName,                                  
                                      body: body);               
        }

        public async ValueTask DisposeAsync()
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
            await _channel.DisposeAsync();
            await _connection.DisposeAsync();           
        }
    }
}
