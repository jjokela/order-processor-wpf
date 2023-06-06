using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Enums;

namespace OrderProcessor.Test.Domain.Entities
{
    public class OrderBookTests
    {
        private Order _buyOrder1 = null!;
        private Order _sellOrder1 = null!;
        private Order _sellOrder2 = null!;
        private Order _buyOrder2 = null!;
        private OrderBook _orderBook = null!;

        [SetUp]
        public void Setup()
        {
            _buyOrder1 = new Order { OrderId = 1, Price = 100, Side = OrderSide.Buy, Size = 50, Symbol = "NVD" };
            _buyOrder2 = new Order { OrderId = 2, Price = 100, Side = OrderSide.Buy, Size = 60, Symbol = "NVD" };
            _sellOrder1 = new Order { OrderId = 3, Price = 100, Side = OrderSide.Sell, Size = 50, Symbol = "NVD" };
            _sellOrder2 = new Order { OrderId = 4, Price = 100, Side = OrderSide.Sell, Size = 70, Symbol = "NVD" };

            _orderBook = new OrderBook("NVD", 5);
        }

        [Test]
        public void AddBuyOrder_NewPriceLevel_OrderAdded()
        {
            _orderBook.AddOrder(_buyOrder1, 1);

            var buySideOrders = _orderBook.BuyOrders;
            var sellSideOrders = _orderBook.SellOrders;

            Assert.Multiple(() =>
            {
                Assert.That(buySideOrders.Count, Is.EqualTo(1));
                Assert.That(sellSideOrders.Count, Is.EqualTo(0));

                var firstBuySideOrder = buySideOrders.First();

                Assert.That(firstBuySideOrder.Price, Is.EqualTo(_buyOrder1.Price));
                Assert.That(firstBuySideOrder.PriceLevelCount, Is.EqualTo(1));
                Assert.That(firstBuySideOrder.PriceLevelQuantity, Is.EqualTo(50));
            });
        }

        [Test]
        public void AddBuyOrder_ExistingPriceLevel_OrderAdded()
        {
            _orderBook.AddOrder(_buyOrder1, 1);
            _orderBook.AddOrder(_buyOrder2, 2);

            var buySideOrders = _orderBook.BuyOrders;
            var sellSideOrders = _orderBook.SellOrders;

            Assert.Multiple(() =>
            {
                Assert.That(buySideOrders.Count, Is.EqualTo(1));
                Assert.That(sellSideOrders.Count, Is.EqualTo(0));

                var lastBuySideOrder = buySideOrders.Last();

                Assert.That(lastBuySideOrder.Price, Is.EqualTo(_buyOrder2.Price));
                Assert.That(lastBuySideOrder.PriceLevelCount, Is.EqualTo(2));
                Assert.That(lastBuySideOrder.PriceLevelQuantity, Is.EqualTo(110));
            });
        }

        [Test]
        public void AddSellOrder_NewPriceLevel_OrderAdded()
        {
            _orderBook.AddOrder(_sellOrder1, 3);

            var buySideOrders = _orderBook.BuyOrders;
            var sellSideOrders = _orderBook.SellOrders;

            Assert.Multiple(() =>
            {
                Assert.That(buySideOrders.Count, Is.EqualTo(0));
                Assert.That(sellSideOrders.Count, Is.EqualTo(1));

                var firstSellSideOrder = sellSideOrders.First();

                Assert.That(firstSellSideOrder.Price, Is.EqualTo(_sellOrder1.Price));
                Assert.That(firstSellSideOrder.PriceLevelCount, Is.EqualTo(1));
                Assert.That(firstSellSideOrder.PriceLevelQuantity, Is.EqualTo(50));
            });
        }

        [Test]
        public void AddSellOrder_ExistingPriceLevel_OrderAdded()
        {
            _orderBook.AddOrder(_sellOrder1, 3);
            _orderBook.AddOrder(_sellOrder2, 4);

            var buySideOrders = _orderBook.BuyOrders;
            var sellSideOrders = _orderBook.SellOrders;

            Assert.Multiple(() =>
            {
                Assert.That(buySideOrders.Count, Is.EqualTo(0));
                Assert.That(sellSideOrders.Count, Is.EqualTo(1));

                var lastSellSideOrder = sellSideOrders.Last();

                Assert.That(lastSellSideOrder.Price, Is.EqualTo(_sellOrder2.Price));
                Assert.That(lastSellSideOrder.PriceLevelCount, Is.EqualTo(2));
                Assert.That(lastSellSideOrder.PriceLevelQuantity, Is.EqualTo(120));
            });
        }

        [Test]
        public void AddBuyOrder_NewPriceLevel_OrderAddedToLookup()
        {
            _orderBook.AddOrder(_buyOrder1, 1);
            var orderLookup = _orderBook.BuyOrderLookup;

            Assert.That(orderLookup.Count, Is.EqualTo(1));

            var firstLookupEntry = orderLookup.First();
            Assert.That(firstLookupEntry.Key, Is.EqualTo(_buyOrder1.OrderId));

            var (order, priceLevel) = firstLookupEntry.Value;

            Assert.Multiple(() =>
            {
                Assert.That(order.OrderId, Is.EqualTo(_buyOrder1.OrderId));
                Assert.That(priceLevel.Price, Is.EqualTo(100));
                Assert.That(priceLevel.PriceLevelCount, Is.EqualTo(1));
                Assert.That(priceLevel.PriceLevelQuantity, Is.EqualTo(50));
            });
        }

        [Test]
        public void AddBuyOrder_ExistingPriceLevel_OrderAddedToLookup()
        {
            _orderBook.AddOrder(_buyOrder1, 1);
            _orderBook.AddOrder(_buyOrder2, 2);
            var orderLookup = _orderBook.BuyOrderLookup;

            Assert.That(orderLookup.Count, Is.EqualTo(2));

            var lastLookupEntry = orderLookup.Last();
            Assert.That(lastLookupEntry.Key, Is.EqualTo(2));

            var (order, priceLevel) = lastLookupEntry.Value;

            Assert.Multiple(() =>
            {
                Assert.That(order.OrderId, Is.EqualTo(_buyOrder2.OrderId));
                Assert.That(priceLevel.Price, Is.EqualTo(100));
                Assert.That(priceLevel.PriceLevelCount, Is.EqualTo(2));
                Assert.That(priceLevel.PriceLevelQuantity, Is.EqualTo(110));
            });
        }

        [Test]
        public void AddSellOrder_NewPriceLevel_OrderAddedToLookup()
        {
            _orderBook.AddOrder(_sellOrder1, 3);
            var orderLookup = _orderBook.SellOrderLookup;

            Assert.That(orderLookup.Count, Is.EqualTo(1));

            var firstLookupEntry = orderLookup.First();
            Assert.That(firstLookupEntry.Key, Is.EqualTo(_sellOrder1.OrderId));

            var (order, priceLevel) = firstLookupEntry.Value;

            Assert.Multiple(() =>
            {
                Assert.That(order.OrderId, Is.EqualTo(_sellOrder1.OrderId));
                Assert.That(priceLevel.Price, Is.EqualTo(100));
                Assert.That(priceLevel.PriceLevelCount, Is.EqualTo(1));
                Assert.That(priceLevel.PriceLevelQuantity, Is.EqualTo(50));
            });
        }

        [Test]
        public void AddSellOrder_ExistingPriceLevel_OrderAddedToLookup()
        {
            _orderBook.AddOrder(_sellOrder1, 3);
            _orderBook.AddOrder(_sellOrder2, 4);
            var orderLookup = _orderBook.SellOrderLookup;

            Assert.That(orderLookup.Count, Is.EqualTo(2));

            var lastLookupEntry = orderLookup.Last();
            Assert.That(lastLookupEntry.Key, Is.EqualTo(_sellOrder2.OrderId));

            var (order, priceLevel) = lastLookupEntry.Value;

            Assert.Multiple(() =>
            {
                Assert.That(order.OrderId, Is.EqualTo(_sellOrder2.OrderId));
                Assert.That(priceLevel.Price, Is.EqualTo(100));
                Assert.That(priceLevel.PriceLevelCount, Is.EqualTo(2));
                Assert.That(priceLevel.PriceLevelQuantity, Is.EqualTo(120));
            });
        }

        [Test]
        public void AddOrder_NullOrder_ThrowsArgumentNullException()
        {
            Order nullOrder = null!;

            Assert.Throws<ArgumentNullException>(() => _orderBook.AddOrder(nullOrder, 0));
        }

        [Test]
        public void AddOrder_DuplicateOrderId_ThrowsArgumentException()
        {
            var duplicateOrder = new Order { OrderId = 1, Symbol = "NVD", Side = OrderSide.Buy, Size = 100, Price = 150 };

            _orderBook.AddOrder(_buyOrder1, 1);

            Assert.Throws<ArgumentException>(() => _orderBook.AddOrder(duplicateOrder, 0));
        }

        [Test]
        public void ExecuteOrder_OrderDoesNotExist_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => _orderBook.ExecuteOrder(123, OrderSide.Buy, 10, 0));
        }

        [Test]
        public void ExecuteOrder_TradeQuantityMoreThanOrderQuantity_ThrowsException()
        {
            _orderBook.AddOrder(_buyOrder1, 1);
            Assert.Throws<ArgumentException>(() => _orderBook.ExecuteOrder(1, OrderSide.Buy, 100000, 0));
        }

        [Test]
        public void ExecuteBuyOrder_ValidOrderAndQuantity_DecreasesQuantity()
        {
            _orderBook.AddOrder(_buyOrder1, 1);
            _orderBook.ExecuteOrder(_buyOrder1.OrderId, OrderSide.Buy, 25, 2);

            var updatedOrder = _orderBook.BuyOrderLookup[_buyOrder1.OrderId].Item1;
            Assert.That(updatedOrder.Size, Is.EqualTo(25));
            var updatedPriceLevel = _orderBook.BuyOrderLookup[_buyOrder1.OrderId].Item2;
            Assert.That(updatedPriceLevel.PriceLevelCount, Is.EqualTo(1));
            Assert.That(updatedPriceLevel.PriceLevelQuantity, Is.EqualTo(25));
        }

        [Test]
        public void ExecuteBuyOrder_ValidOrderAndQuantity_DecreasesQuantityToZero_AndRemovesFromLookup()
        {
            _orderBook.AddOrder(_buyOrder1, 1);
            _orderBook.ExecuteOrder(_buyOrder1.OrderId, OrderSide.Buy, 50, 2);

            Assert.That(_orderBook.BuyOrderLookup.Count, Is.EqualTo(0));
        }

        [Test]
        public void ExecuteSellOrder_ValidOrderAndQuantity_DecreasesQuantity()
        {
            _orderBook.AddOrder(_sellOrder2, 4);
            _orderBook.ExecuteOrder(_sellOrder2.OrderId, OrderSide.Sell, 25, 5);

            var updatedOrder = _orderBook.SellOrderLookup[_sellOrder2.OrderId].Item1;
            Assert.That(updatedOrder.Size, Is.EqualTo(45));
            var updatedPriceLevel = _orderBook.SellOrderLookup[_sellOrder2.OrderId].Item2;
            Assert.That(updatedPriceLevel.PriceLevelCount, Is.EqualTo(1));
            Assert.That(updatedPriceLevel.PriceLevelQuantity, Is.EqualTo(45));
        }

        [Test]
        public void ExecuteSellOrder_ValidOrderAndQuantity_DecreasesQuantityToZero_AndRemovesFromLookup()
        {
            _orderBook.AddOrder(_sellOrder1, 3);
            _orderBook.ExecuteOrder(_sellOrder1.OrderId, OrderSide.Sell, 50, 4);

            Assert.That(_orderBook.SellOrderLookup.Count, Is.EqualTo(0));
        }

        [Test]
        public void DeleteOrder_OrderDoesNotExist_ThrowsException()
        {
            var nonExistentOrderId = 999UL;

            Assert.Throws<ArgumentException>(() => _orderBook.DeleteOrder(nonExistentOrderId, OrderSide.Buy, 0));
        }

        [Test]
        public void DeleteBuyOrder_OrderExists_OrderRemoved()
        {
            _orderBook.AddOrder(_buyOrder1, 1);

            _orderBook.DeleteOrder(_buyOrder1.OrderId, OrderSide.Buy, 2);

            Assert.False(_orderBook.BuyOrderLookup.ContainsKey(_buyOrder1.OrderId));
            Assert.That(_orderBook.BuyOrders.Count, Is.EqualTo(0));
        }

        [Test]
        public void DeleteSellOrder_OrderExists_OrderRemoved()
        {
            _orderBook.AddOrder(_sellOrder1, 3);

            _orderBook.DeleteOrder(_sellOrder1.OrderId, OrderSide.Sell, 4);

            Assert.False(_orderBook.SellOrderLookup.ContainsKey(_sellOrder1.OrderId));
            Assert.That(_orderBook.SellOrderLookup.Count, Is.EqualTo(0));
        }

        [Test]
        public void DeleteBuyOrder_DecreasesPriceLevelCountAndQuantity()
        {
            _orderBook.AddOrder(_buyOrder1, 1);
            _orderBook.AddOrder(_buyOrder2, 2);

            _orderBook.DeleteOrder(_buyOrder1.OrderId, OrderSide.Buy, 3);

            Assert.That(_orderBook.BuyOrders.Count, Is.EqualTo(1));

            var priceLevel = _orderBook.BuyOrders.First();
            Assert.That(priceLevel.PriceLevelCount, Is.EqualTo(1));
            Assert.That(priceLevel.PriceLevelQuantity, Is.EqualTo(60));
        }

        [Test]
        public void DeleteSellOrder_DecreasesPriceLevelCountAndQuantity()
        {
            _orderBook.AddOrder(_sellOrder1, 3);
            _orderBook.AddOrder(_sellOrder2, 4);

            _orderBook.DeleteOrder(_sellOrder1.OrderId, OrderSide.Sell, 5);

            Assert.That(_orderBook.SellOrders.Count, Is.EqualTo(1));

            var priceLevel = _orderBook.SellOrders.First();
            Assert.That(priceLevel.PriceLevelCount, Is.EqualTo(1));
            Assert.That(priceLevel.PriceLevelQuantity, Is.EqualTo(70));
        }

        [Test]
        public void UpdateOrder_OrderDoesNotExist_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => _orderBook.UpdateOrder(_buyOrder1, 1));
        }

        [Test]
        public void UpdateOrderSize_OrderExists_OrderUpdated()
        {
            _orderBook.AddOrder(_buyOrder1, 1);
            _orderBook.AddOrder(_buyOrder2, 2);

            var updatedOrder = new Order
            {
                OrderId = _buyOrder1.OrderId,
                Price = _buyOrder1.Price,
                Side = _buyOrder1.Side,
                Size = 1000,
                Symbol = _buyOrder1.Symbol
            };

            _orderBook.UpdateOrder(updatedOrder, 3);

            Assert.True(_orderBook.BuyOrderLookup.ContainsKey(_buyOrder1.OrderId));
            var priceLevel = _orderBook.BuyOrders.First();
            Assert.That(priceLevel.PriceLevelCount, Is.EqualTo(2));
            Assert.That(priceLevel.PriceLevelQuantity, Is.EqualTo(1060));
        }

        [Test]
        public void UpdateOrderPrice_OrderExists_OrderUpdated()
        {
            _orderBook.AddOrder(_buyOrder1, 1);

            var updatedOrder = new Order
            {
                OrderId = _buyOrder1.OrderId,
                Price = 1000,
                Side = _buyOrder1.Side,
                Size = _buyOrder1.Size,
                Symbol = _buyOrder1.Symbol
            };

            _orderBook.UpdateOrder(updatedOrder, 2);

            Assert.True(_orderBook.BuyOrderLookup.ContainsKey(_buyOrder1.OrderId));
            Assert.That(_orderBook.BuyOrders.Count, Is.EqualTo(1));
            var priceLevel = _orderBook.BuyOrders.First();
            Assert.That(priceLevel.Price, Is.EqualTo(1000));
            Assert.That(priceLevel.PriceLevelCount, Is.EqualTo(1));
            Assert.That(priceLevel.PriceLevelQuantity, Is.EqualTo(50));
        }

        [Test]
        public void ExecuteOrder_WhenOrderIsFullyExecuted_RemovesPriceLevelIfQuantityIsZero()
        {
            _orderBook.AddOrder(_buyOrder1, 1);

            _orderBook.ExecuteOrder(_buyOrder1.OrderId, _buyOrder1.Side, _buyOrder1.Size, 2);

            Assert.That(_orderBook.BuyOrders.Count, Is.EqualTo(0));
        }

        [Test]
        public void GetSnapshot_WhenSnapshotIsUpdated_ReturnsSnapshot()
        {
            _orderBook.AddOrder(_buyOrder1, 1);

            var snapshot = _orderBook.GetSnapshot();

            Assert.That(snapshot, Is.Not.Null);
            Assert.That(snapshot!.IsUpdated, Is.True);
        }

        [Test]
        public void UpdateSnapshot_WhenPriceIsInVisibleRange_UpdatesSnapshot()
        {
            _orderBook.AddOrder(_buyOrder1, 1);

            var snapshot = _orderBook.GetSnapshot();

            Assert.That(snapshot, Is.Not.Null);
            Assert.That(snapshot.SequenceNumber, Is.EqualTo(1));
            Assert.That(snapshot.BidBook.Count, Is.EqualTo(1));
        }
    }
}