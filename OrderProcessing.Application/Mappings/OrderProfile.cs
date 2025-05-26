using AutoMapper;
using OrderProcessing.Application.Commands.CreateOrder;
using OrderProcessing.Application.DTOs;
using OrderProcessing.Domain.Entities;

namespace OrderProcessing.Application.Mappings;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<CreateOrderCommand, Order>();
        CreateMap<Order, OrderResponse>();
    }
}
