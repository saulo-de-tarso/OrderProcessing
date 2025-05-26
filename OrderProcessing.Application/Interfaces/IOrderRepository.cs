using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Application.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<Order?> GetByIdAsync(Guid id);
    Task UpdateAsync(Order order);

}
