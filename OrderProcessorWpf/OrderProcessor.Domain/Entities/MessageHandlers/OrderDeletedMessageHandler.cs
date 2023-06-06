using OrderProcessor.Domain.Entities.Messages;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Domain.Entities.MessageHandlers
{
    public class OrderDeletedMessageHandler : OrderMessageHandlerBase
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

            if (orderMessage is not OrderMessageDeleted orderMessageDeleted)
            {
                throw new ArgumentException("Order message must be of type OrderMessageAdded.");
            }

            orderBook.DeleteOrder(orderMessageDeleted.OrderId, orderMessageDeleted.Side, orderMessageDeleted.SequenceNumber);
        }
    }
}