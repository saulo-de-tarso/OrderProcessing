namespace OrderProcessing.Infrastructure.MessageBroker;

public class RabbitMqConfiguration
{

    public const string SectionName = "RabbitMq";
    
    public string HostName { get; set; }
    public int Port { get; set; } = 5672; // Default RabbitMQ port
    public string UserName { get; set; }
    public string Password { get; set; }
    public string VirtualHost { get; set; } = "/";
    public string QueueName { get; set; }

}
