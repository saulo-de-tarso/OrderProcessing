using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Entities;
using OrderProcessing.Infrastructure.MessageBroker;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;


public sealed class RabbitMqMessageBroker : IMessageBroker, IAsyncDisposable
{

    private readonly ILogger<RabbitMqMessageBroker> _logger;
    private readonly RabbitMqConfiguration _config;
    private readonly SemaphoreSlim _initLock = new(1, 1);

    private IConnection? _connection;
    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;
    private bool _initialized;
    private bool _disposed;
    

    public RabbitMqMessageBroker(IOptions<RabbitMqConfiguration> config,
        ILogger<RabbitMqMessageBroker> logger)
    {
        _config = config.Value;
        _logger = logger;
    }

    private ConnectionFactory CreateConnectionFactory()
    {
        return new ConnectionFactory
        {
            HostName = _config.HostName,
            Port = _config.Port,
            UserName = _config.UserName,
            Password = _config.Password,
            VirtualHost = _config.VirtualHost,
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };
    }

    public async Task InitializeAsync()
    {
        if (_initialized) return;

        await _initLock.WaitAsync();
        try
        {
            if (_initialized) return;

            var factory = CreateConnectionFactory();
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync(
                queue: _config.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            
            _initialized = true;
            _logger.LogInformation("RabbitMQ connection established.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ initialization failed.");
            throw;
        }
        finally { _initLock.Release(); }
        
    }

    public async Task PublishOrderAsync(Order order)
    {
        if (!_initialized)
            throw new InvalidOperationException("Broker not initialized. Call InitializeAsync first.");
        try
        {
            var message = JsonSerializer.Serialize(order);
            var body = Encoding.UTF8.GetBytes(message);
            
            var properties = new BasicProperties
            {
                DeliveryMode = DeliveryModes.Persistent
            };

            await _channel!.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: _config.QueueName,
                mandatory: false,
                basicProperties: properties,
                body: body);

            _logger.LogInformation($"Published order {order.Id}");

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to publish {order.Id}.");
            throw;
        }
    }

    public async Task ConsumeOrdersAsync(Func<Order, Task> processOrder)
    {
        if (!_initialized)
            throw new InvalidOperationException("Broker not initialized. Call InitializeAsync first.");

        _consumer = new AsyncEventingBasicConsumer(_channel);

        _consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                var order = JsonSerializer.Deserialize<Order>(ea.Body.Span);
                if (order != null)
                {
                    await processOrder(order);
                    await _channel.BasicAckAsync(ea.DeliveryTag, false);
                    _logger.LogInformation($"Processed order {order.Id}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing order.");
                await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
               
            }
        };

        await _channel!.BasicConsumeAsync(
            queue: _config.QueueName,
            autoAck: false,
            consumer: _consumer);
        _logger.LogInformation($"Started consuming from queue {_config.QueueName}");
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        try
        {
            if (_channel?.IsOpen == true)
            {
                await _channel.CloseAsync();
                _channel.Dispose();
            }

            if (_connection?.IsOpen == true)
            {
                await _connection.CloseAsync();
                _connection.Dispose();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during disposal");
        }
        finally
        {
            _disposed = true;
            _initLock.Dispose();
            GC.SuppressFinalize(this);
        }
    }

}
