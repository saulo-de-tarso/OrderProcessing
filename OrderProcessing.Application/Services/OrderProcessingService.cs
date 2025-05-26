using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Enums;

namespace OrderProcessing.Application.Services;

public class OrderProcessingService(IMessageBroker messageBroker,
    IOrderRepository orderRepository,
    ILogger<OrderProcessingService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Order processing service started.");

        await messageBroker.ConsumeOrdersAsync(async order =>
        {
            try
            {
                logger.LogInformation($"Processing order {order.Id}");

                await Task.Delay(2000, cancellationToken);

                order.Status = OrderStatus.Processed;

                await orderRepository.UpdateAsync(order);

                logger.LogInformation($"Order {order.Id} processed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to process order {order.Id}");
                throw;
            }
        });
    }
}
