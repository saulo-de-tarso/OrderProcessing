using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Application.Interfaces;

public interface IOrderRepository
{
    void Add(Order order);
    Order? GetById(Guid id);
    void Update(Order order);

}
