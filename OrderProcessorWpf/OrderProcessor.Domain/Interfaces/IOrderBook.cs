using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Enums;

namespace OrderProcessor.Domain.Interfaces;

public interface IOrderBook
{
    void AddOrder(Order order, uint sequenceNumber);
    void ExecuteOrder(ulong orderId, OrderSide orderSide, ulong tradedQuantity, uint sequenceNumber);
    void DeleteOrder(ulong orderId, OrderSide orderSide, uint sequenceNumber);
    void UpdateOrder(Order order, uint sequenceNumber);
}