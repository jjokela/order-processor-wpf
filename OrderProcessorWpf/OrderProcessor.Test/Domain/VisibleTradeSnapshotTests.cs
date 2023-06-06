using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Enums;
using OrderProcessor.Presentation.Services;

namespace OrderProcessor.Test.Domain
{
    /// <summary>
    /// Seed the OrderBook with trades, and verify that different operations result to a correct visible snapshot
    /// </summary>
    public class VisibleTradeSnapshotTests
    {
        private Order _buyOrder1 = null!;
        private Order _buyOrder2 = null!;
        private Order _buyOrder3 = null!;
        private Order _buyOrder4 = null!;

        private Order _sellOrder1 = null!;
        private Order _sellOrder2 = null!;
        private Order _sellOrder3 = null!;
        private Order _sellOrder4 = null!;
        private OrderBook _orderBook = null!;

        [SetUp]
        public void Setup()
        {
            _buyOrder1 = new Order { OrderId = 1, Price = 110, Side = OrderSide.Buy, Size = 200, Symbol = "NVD" };
            _buyOrder2 = new Order { OrderId = 2, Price = 100, Side = OrderSide.Buy, Size = 150, Symbol = "NVD" };
            _buyOrder3 = new Order { OrderId = 3, Price = 90, Side = OrderSide.Buy, Size = 100, Symbol = "NVD" };
            _buyOrder4 = new Order { OrderId = 4, Price = 80, Side = OrderSide.Buy, Size = 50, Symbol = "NVD" };
            _sellOrder1 = new Order { OrderId = 5, Price = 120, Side = OrderSide.Sell, Size = 200, Symbol = "NVD" };
            _sellOrder2 = new Order { OrderId = 6, Price = 130, Side = OrderSide.Sell, Size = 150, Symbol = "NVD" };
            _sellOrder3 = new Order { OrderId = 7, Price = 140, Side = OrderSide.Sell, Size = 100, Symbol = "NVD" };
            _sellOrder4 = new Order { OrderId = 8, Price = 150, Side = OrderSide.Sell, Size = 50, Symbol = "NVD" };

            _orderBook = new OrderBook("NVD", 2);
            _orderBook.AddOrder(_buyOrder1, 1);
            _orderBook.AddOrder(_buyOrder2, 2);
            _orderBook.AddOrder(_buyOrder3, 3);
            _orderBook.AddOrder(_buyOrder4, 4);
            _orderBook.AddOrder(_sellOrder1, 5);
            _orderBook.AddOrder(_sellOrder2, 6);
            _orderBook.AddOrder(_sellOrder3, 7);
            _orderBook.AddOrder(_sellOrder4, 8);
        }

        [Test]
        public void AddOrders_ShowsCorrectResult()
        {
            var newOrders = new[]
            {
                new Order { OrderId = 11, Price = 105, Side = OrderSide.Buy, Size = 100, Symbol = "NVD" },
                new Order { OrderId = 12, Price = 85, Side = OrderSide.Buy, Size = 100, Symbol = "NVD" },
                new Order { OrderId = 13, Price = 125, Side = OrderSide.Sell, Size = 100, Symbol = "NVD" },
                new Order { OrderId = 14, Price = 145, Side = OrderSide.Sell, Size = 100, Symbol = "NVD" }
            };

            var snapshots = new Snapshot[newOrders.Length];

            for (var i = 0; i < newOrders.Length; i++)
            {
                _orderBook.AddOrder(newOrders[i], (uint)(100 + i));
                snapshots[i] = _orderBook.GetSnapshot()!;
            }

            const string expected1 = "100, NVD, [(110, 200), (105, 100)], [(120, 200), (130, 150)]";
            const string expected3 = "102, NVD, [(110, 200), (105, 100)], [(120, 200), (125, 100)]";

            Assert.Multiple(() =>
            {
                Assert.That(SnapshotFormattingService.FormatSnapshot(snapshots[0]), Is.EqualTo(expected1));
                Assert.IsNull(snapshots[1]);
                Assert.That(SnapshotFormattingService.FormatSnapshot(snapshots[2]), Is.EqualTo(expected3));
                Assert.IsNull(snapshots[3]);
            });
        }

        [Test]
        public void ExecuteOrders_ShowsCorrectResult()
        {
            var orders = new[]
            {
                _buyOrder2,
                _buyOrder4,
                _sellOrder2,
                _sellOrder3,
            };

            var snapshots = new Snapshot?[orders.Length];

            for (var i = 0; i < orders.Length; i++)
            {
                var order = orders[i];
                _orderBook.ExecuteOrder(order.OrderId, order.Side, 10, (uint)(101 + i));
                snapshots[i] = _orderBook.GetSnapshot();
            }

            Assert.Multiple(() =>
            {
                Assert.That(SnapshotFormattingService.FormatSnapshot(snapshots[0]!), Is.EqualTo("101, NVD, [(110, 200), (100, 140)], [(120, 200), (130, 150)]"));
                Assert.IsNull(snapshots[1]);
                Assert.That(SnapshotFormattingService.FormatSnapshot(snapshots[2]!), Is.EqualTo("103, NVD, [(110, 200), (100, 140)], [(120, 200), (130, 140)]"));
                Assert.IsNull(snapshots[3]);
            });
        }

        [Test]
        public void UpdateOrders_ShowsCorrectResult()
        {
            var newOrders = new[]
            {
                new Order { OrderId = 2, Price = 105, Side = OrderSide.Buy, Size = 100, Symbol = "NVD" },
                new Order { OrderId = 3, Price = 95, Side = OrderSide.Buy, Size = 100, Symbol = "NVD" },
                new Order { OrderId = 6, Price = 125, Side = OrderSide.Sell, Size = 100, Symbol = "NVD" },
                new Order { OrderId = 7, Price = 145, Side = OrderSide.Sell, Size = 100, Symbol = "NVD" }
            };

            var snapshots = new Snapshot[newOrders.Length];

            for (var i = 0; i < newOrders.Length; i++)
            {
                _orderBook.UpdateOrder(newOrders[i], (uint)(100 + i));
                snapshots[i] = _orderBook.GetSnapshot()!;
            }

            const string expected1 = "100, NVD, [(110, 200), (105, 100)], [(120, 200), (130, 150)]";
            const string expected3 = "102, NVD, [(110, 200), (105, 100)], [(120, 200), (125, 100)]";

            Assert.Multiple(() =>
            {
                Assert.That(SnapshotFormattingService.FormatSnapshot(snapshots[0]), Is.EqualTo(expected1));
                Assert.IsNull(snapshots[1]);
                Assert.That(SnapshotFormattingService.FormatSnapshot(snapshots[2]), Is.EqualTo(expected3));
                Assert.IsNull(snapshots[3]);
            });
        }

        [Test]
        public void DeleteOrders_ShowsCorrectResult()
        {
            var orderIdAndSidePairs = new[]
            {
                (_buyOrder2.OrderId, _buyOrder2.Side),
                (_buyOrder3.OrderId, _buyOrder3.Side),
                (_sellOrder2.OrderId, _sellOrder2.Side),
                (_sellOrder3.OrderId, _sellOrder3.Side),
            };

            var snapshots = new Snapshot[orderIdAndSidePairs.Length];

            for (var i = 0; i < orderIdAndSidePairs.Length; i++)
            {
                _orderBook.DeleteOrder(orderIdAndSidePairs[i].OrderId, orderIdAndSidePairs[i].Side, (uint)(100 + i));
                snapshots[i] = _orderBook.GetSnapshot()!;
            }

            const string expected1 = "100, NVD, [(110, 200), (90, 100)], [(120, 200), (130, 150)]";
            const string expected2 = "101, NVD, [(110, 200), (80, 50)], [(120, 200), (130, 150)]";
            const string expected3 = "102, NVD, [(110, 200), (80, 50)], [(120, 200), (140, 100)]";
            const string expected4 = "103, NVD, [(110, 200), (80, 50)], [(120, 200), (150, 50)]";

            Assert.Multiple(() =>
            {
                Assert.That(SnapshotFormattingService.FormatSnapshot(snapshots[0]), Is.EqualTo(expected1));
                Assert.That(SnapshotFormattingService.FormatSnapshot(snapshots[1]), Is.EqualTo(expected2));
                Assert.That(SnapshotFormattingService.FormatSnapshot(snapshots[2]), Is.EqualTo(expected3));
                Assert.That(SnapshotFormattingService.FormatSnapshot(snapshots[3]), Is.EqualTo(expected4));
            });
        }
    }
}