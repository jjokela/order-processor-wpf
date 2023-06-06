using OrderProcessor.Domain.Entities.Messages;
using OrderProcessor.Domain.Enums;

namespace OrderProcessor.Domain.Entities
{
    public class Order
    {
        public ulong OrderId { get; init; }
        public string Symbol { get; init; } = string.Empty;
        public OrderSide Side { get; init; }
        public ulong Size { get; set; }
        public int Price { get; set; }

        public static Order CreateFromMessage(OrderMessageAdded message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), $"{nameof(message)} cannot be null.");
            }

            return new Order
            {
                OrderId = message.OrderId,
                Price = message.Price,
                Side = message.Side,
                Size = message.Size,
                Symbol = message.Symbol
            };
        }

        public static Order CreateFromMessage(OrderMessageUpdated message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), $"{nameof(message)} cannot be null.");
            }

            return new Order
            {
                OrderId = message.OrderId,
                Price = message.Price,
                Side = message.Side,
                Size = message.Size,
                Symbol = message.Symbol
            };
        }
    }
}
