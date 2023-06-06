using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Domain.Services;

public class OrderMessageDispatcher
{
    private readonly Dictionary<Type, OrderMessageHandlerBase> _orderMessageHandlers;

    public OrderMessageDispatcher(Dictionary<Type, OrderMessageHandlerBase> orderMessageHandlers)
    {
        _orderMessageHandlers = orderMessageHandlers;
    }

    public Snapshot? Dispatch(OrderMessageBase order, OrderBook orderBook)
    {
        if (order == null)
        {
            throw new ArgumentNullException(nameof(order));
        }

        if (orderBook == null)
        {
            throw new ArgumentNullException(nameof(orderBook));
        }

        var messageType = order.GetType();
        if (_orderMessageHandlers.TryGetValue(messageType, out var handler))
        {
            handler.HandleOrderMessage(order, orderBook);
            return handler.GetSnapshot(orderBook);
        }

        throw new NotSupportedException($"Order type {messageType.Name} is not supported.");
    }
}