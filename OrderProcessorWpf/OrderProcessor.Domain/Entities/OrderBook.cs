using OrderProcessor.Domain.Comparers;
using OrderProcessor.Domain.Enums;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Domain.Entities
{
    public class OrderBook : IOrderBook
    {
        public string Symbol { get; }
        public SortedSet<PriceLevel> BuyOrders { get; } = new(new BuyPriceLevelComparer());
        public SortedSet<PriceLevel> SellOrders { get; } = new(new SellPriceLevelComparer());
        public Dictionary<ulong, (Order, PriceLevel)> BuyOrderLookup { get; } = new();
        public Dictionary<ulong, (Order, PriceLevel)> SellOrderLookup { get; } = new();

        private readonly Snapshot _snapshot;
        private readonly uint _depth;

        public OrderBook(string symbol, uint depth)
        {
            Symbol = symbol;
            _depth = depth;

            _snapshot = new Snapshot {Symbol = symbol, Depth = depth};
        }
        
        public void AddOrder(Order order, uint sequenceNumber)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var orderLookup = order.Side == OrderSide.Buy ? BuyOrderLookup : SellOrderLookup;
            var ordersBookSide = order.Side == OrderSide.Buy ? BuyOrders : SellOrders;

            if (orderLookup.ContainsKey(order.OrderId))
            {
                throw new ArgumentException($"Trying to add an already existing order as a new order, OrderId: {order.OrderId}");
            }

            var priceLevel = new PriceLevel(order.Price);

            if (!ordersBookSide.TryGetValue(priceLevel, out var existingPriceLevel))
            {
                ordersBookSide.Add(priceLevel);
            }
            else
            {
                priceLevel = existingPriceLevel;
            }

            priceLevel.AddOrder(order);

            orderLookup[order.OrderId] = (order, priceLevel);

            UpdateSnapshot(sequenceNumber, new[] { order.Price }, order.Side);
        }

        public void ExecuteOrder(ulong orderId, OrderSide orderSide, ulong tradedQuantity, uint sequenceNumber)
        {
            var orderLookup = orderSide == OrderSide.Buy ? BuyOrderLookup : SellOrderLookup;
            var ordersBookSide = orderSide == OrderSide.Buy ? BuyOrders : SellOrders;

            if (!orderLookup.TryGetValue(orderId, out var orderTuple))
            {
                throw new ArgumentException($"Order with OrderId {orderId} not found");
            }

            var (order, priceLevel) = orderTuple;
            
            if (order.Size < tradedQuantity)
            {
                throw new ArgumentException($"Traded Quantity {tradedQuantity} is more than existing quantity {order.Size} for order {orderId}");
            }

            priceLevel.UpdateOrderTradedQuantity(tradedQuantity);

            order.Size -= tradedQuantity;

            if (order.Size == 0)
            {
                priceLevel.RemoveOrder(order);
                orderLookup.Remove(orderId);
            }

            if (priceLevel.PriceLevelQuantity == 0)
            {
                ordersBookSide.Remove(priceLevel);
            }

            UpdateSnapshot(sequenceNumber, new[] { order.Price }, order.Side);
        }

        public void DeleteOrder(ulong orderId, OrderSide orderSide, uint sequenceNumber)
        {
            var orderLookup = orderSide == OrderSide.Buy ? BuyOrderLookup : SellOrderLookup;

            var ordersBookSide = orderSide == OrderSide.Buy ? BuyOrders : SellOrders;

            if (!orderLookup.TryGetValue(orderId, out var orderTuple))
            {
                throw new ArgumentException($"Order with OrderId {orderId} not found");
            }

            var (order, priceLevel) = orderTuple;

            priceLevel.RemoveOrder(order);

            orderLookup.Remove(orderId);

            if (priceLevel.PriceLevelQuantity == 0)
            {
                ordersBookSide.Remove(priceLevel);
            }

            UpdateSnapshot(sequenceNumber, new[] { order.Price }, order.Side);
        }

        public void UpdateOrder(Order order, uint sequenceNumber)
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            var orderLookup = order.Side == OrderSide.Buy ? BuyOrderLookup : SellOrderLookup;

            if (!orderLookup.TryGetValue(order.OrderId, out var orderTuple))
            {
                throw new ArgumentException($"Order with OrderId {order.OrderId} not found", nameof(order.OrderId));
            }
            var (existingOrder, _) = orderTuple;

            var currentOrderPrice = existingOrder.Price;
            var newOrderPrice = order.Price;

            DeleteOrder(order.OrderId, order.Side, sequenceNumber);

            AddOrder(order, sequenceNumber);

            UpdateSnapshot(sequenceNumber, new []{currentOrderPrice, newOrderPrice}, order.Side);
        }

        public void UpdateSnapshot(uint sequenceNumber, int[] priceLevels, OrderSide orderSide)
        {
            _snapshot.IsUpdated = _snapshot.PriceInVisibleRange(priceLevels, orderSide);

            if (!_snapshot.IsUpdated)
            {
                return;
            }

            var ordersList = orderSide switch
            {
                OrderSide.Buy => BuyOrders,
                OrderSide.Sell => SellOrders,
                _ => throw new ArgumentException($"Invalid order side: {orderSide}"),
            };

            var orders = ordersList
                .Take((int)_depth)
                .Select(priceLevel => (priceLevel.PriceLevelQuantity, priceLevel.Price))
                .ToList();

            _snapshot.UpdateBookEntries(orders, orderSide);
            _snapshot.SequenceNumber = sequenceNumber;
        }

        public Snapshot? GetSnapshot()
        {
            // return a copy of the snapshot
            return _snapshot.IsUpdated ? new Snapshot(_snapshot) : null;
        }
    }
}
