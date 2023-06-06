using OrderProcessor.Domain.Entities;

namespace OrderProcessor.Domain.Interfaces
{
    public abstract class OrderMessageHandlerBase
    {
        public abstract void HandleOrderMessage(OrderMessageBase orderMessage, IOrderBook orderBook);

        public Snapshot? GetSnapshot(OrderBook orderBook)
        {
            if (orderBook is null)
            {
                throw new ArgumentNullException(nameof(orderBook), "Order book cannot be null.");
            }

            return orderBook.GetSnapshot();
        }
    }
}
