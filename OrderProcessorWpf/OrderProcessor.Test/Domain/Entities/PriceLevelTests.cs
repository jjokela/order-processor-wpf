using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Enums;

namespace OrderProcessor.Test.Domain.Entities
{
    public class PriceLevelTests
    {
        private PriceLevel _priceLevel = null!;
        private Order _order1 = null!;
        private Order _order2 = null!;
        private Order _order3 = null!;

        [SetUp]
        public void Setup()
        {
            _priceLevel = new PriceLevel(100);
            
            _order1 = new Order { OrderId = 1, Price = 100, Side = OrderSide.Buy, Size = 50, Symbol = "NVD" };
            _order2 = new Order { OrderId = 2, Price = 100, Side = OrderSide.Buy, Size = 70, Symbol = "NVD" };
            _order3 = new Order { OrderId = 3, Price = 100, Side = OrderSide.Buy, Size = 20, Symbol = "NVD" };
        }

        [Test]
        public void AddOrder_IncreasesCountAndQuantity()
        {
            _priceLevel.AddOrder(_order1);

            Assert.That(_priceLevel.PriceLevelCount, Is.EqualTo(1));
            Assert.That(_priceLevel.PriceLevelQuantity, Is.EqualTo(50));
        }

        [Test]
        public void AddMultipleOrders_IncreasesCountAndQuantity()
        {
            _priceLevel.AddOrder(_order1);
            _priceLevel.AddOrder(_order2);
            _priceLevel.AddOrder(_order3);

            Assert.That(_priceLevel.PriceLevelCount, Is.EqualTo(3));
            Assert.That(_priceLevel.PriceLevelQuantity, Is.EqualTo(140));
        }

        [Test]
        public void RemoveOrder_DecreasesCountAndQuantity()
        {
            _priceLevel.AddOrder(_order1);
            _priceLevel.AddOrder(_order2);
            _priceLevel.AddOrder(_order3);

            _priceLevel.RemoveOrder(_order2);

            Assert.That(_priceLevel.PriceLevelCount, Is.EqualTo(2));
            Assert.That(_priceLevel.PriceLevelQuantity, Is.EqualTo(70));
        }

        [Test]
        public void TryToRemoveNonExistingOrder_WhenNoOrders_CountAndQuantityDoesNotChange()
        {
            _priceLevel.RemoveOrder(_order1);

            Assert.That(_priceLevel.PriceLevelCount, Is.EqualTo(0));
            Assert.That(_priceLevel.PriceLevelQuantity, Is.EqualTo(0));
        }

        [Test]
        public void TryToRemoveNonExistingOrder_WhenExistingOrders_CountAndQuantityDoesNotChange()
        {
            _priceLevel.AddOrder(_order1);
            _priceLevel.RemoveOrder(_order2);

            Assert.That(_priceLevel.PriceLevelCount, Is.EqualTo(1));
            Assert.That(_priceLevel.PriceLevelQuantity, Is.EqualTo(50));
        }

        [Test]
        public void PriceLevel_SupportsNegativePrices()
        {
            var negativePriceLevel = new PriceLevel(-100);
            var negativePriceOrder = new Order { OrderId = 1, Price = -100, Side = OrderSide.Buy, Size = 70, Symbol = "NVD" };

            negativePriceLevel.AddOrder(negativePriceOrder);

            Assert.That(negativePriceLevel.PriceLevelCount, Is.EqualTo(1));
            Assert.That(negativePriceLevel.PriceLevelQuantity, Is.EqualTo(70));
        }

        [Test]
        public void AddOrder_WithDifferentPriceThanPriceLevel_Throws()
        {
            var differentPriceThanPriceLevelOrder = new Order { OrderId = 1, Price = 987, Side = OrderSide.Buy, Size = 100, Symbol = "NVD" };

            Assert.Throws<ArgumentException>(() => _priceLevel.AddOrder(differentPriceThanPriceLevelOrder));
        }

        [Test]
        public void UpdateOrderTradedQuantity_DecreasesBelowZero_ThrowsException()
        {
            Assert.Throws<InvalidOperationException>(() => _priceLevel.UpdateOrderTradedQuantity(100));
        }

        [Test]
        public void UpdateOrderTradedQuantity_DecreasesPriceLevelQuantity()
        {
            _priceLevel.AddOrder(_order1);
            _priceLevel.UpdateOrderTradedQuantity(25);

            Assert.That(_priceLevel.PriceLevelQuantity, Is.EqualTo(25));
        }

        [Test]
        public void ValidateOrderPrice_OrderPriceDoesNotMatch()
        {
            var priceLevel = new PriceLevel(123);
            
            Assert.Throws<ArgumentException>(() => priceLevel.AddOrder(_order1));
        }
    }
}
