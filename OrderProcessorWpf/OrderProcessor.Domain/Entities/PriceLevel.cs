namespace OrderProcessor.Domain.Entities
{
    public class PriceLevel
    {
        public int Price { get; init; }

        private readonly LinkedList<Order> _orders = new();

        public int PriceLevelCount => _orders.Count;

        public ulong PriceLevelQuantity { get; private set; }

        public PriceLevel(int price)
        {
            Price = price;
        }

        public void AddOrder(Order order)
        {
            ValidateOrderPrice(order);

            _orders.AddLast(order);
            PriceLevelQuantity += order.Size;
        }

        public void RemoveOrder(Order order)
        {
            var orderWasRemoved = _orders.Remove(order);

            if (orderWasRemoved)
            {
                PriceLevelQuantity -= order.Size;
            }
        }

        public void UpdateOrderTradedQuantity(ulong tradedQuantityChange)
        {
            if (PriceLevelQuantity < tradedQuantityChange)
            {
                throw new InvalidOperationException("Cannot decrease quantity below zero.");
            }

            PriceLevelQuantity -= tradedQuantityChange;
        }

        public void ValidateOrderPrice(Order order)
        {
            if (order.Price != Price)
            {
                throw new ArgumentException($"Order price {order.Price} doesn't match with the PriceLevel price {Price}");
            }
        }
    }
}