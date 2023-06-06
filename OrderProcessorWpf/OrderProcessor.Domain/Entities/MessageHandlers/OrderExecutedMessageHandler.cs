using OrderProcessor.Domain.Entities.Messages;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Domain.Entities.MessageHandlers
{
    public class OrderExecutedMessageHandler : OrderMessageHandlerBase
    {
        public override void HandleOrderMessage(OrderMessageBase orderMessage, IOrderBook orderBook)
        {
            if (orderMessage is null)
            {
                throw new ArgumentNullException(nameof(orderMessage), "Order message cannot be null.");
            }

            if (orderBook is null)
            {
                throw new ArgumentNullException(nameof(orderBook), "Order book cannot be null.");
            }

            if (orderMessage is not OrderMessageExecuted orderMessageExecuted)
            {
                throw new ArgumentException("Order message must be of type OrderMessageAdded.");
            }

            orderBook.ExecuteOrder(orderMessageExecuted.OrderId, orderMessageExecuted.Side, orderMessageExecuted.TradedQuantity, orderMessageExecuted.SequenceNumber);
        }
    }
}