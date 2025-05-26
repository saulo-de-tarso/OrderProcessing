using OrderProcessing.Application.Interfaces;
using OrderProcessing.Domain.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessing.Infrastructure.Repositories;

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public Task<Order?> GetByIdAsync(Guid id)
    {
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task AddAsync(Order order)
    {
        if(!_orders.TryAdd(order.Id, order))
        {
            throw new InvalidOperationException($"Order with Id {order.Id} already exists.");
        }
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order)
    {
        if (!_orders.TryGetValue(order.Id, out _))
        {
            throw new KeyNotFoundException($"Order with Id {order.Id} not found.");
        }

        _orders[order.Id] = order;
        return Task.CompletedTask;
    }


}
