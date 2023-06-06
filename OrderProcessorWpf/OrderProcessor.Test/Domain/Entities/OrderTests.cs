using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Entities.Messages;
using OrderProcessor.Domain.Enums;

namespace OrderProcessor.Test.Domain.Entities
{
    public class OrderTests
    {
        [Test]
        public void CreateFromMessage_AddedMessage_ShouldReturnOrder()
        {
            var message = new OrderMessageAdded { OrderId = 1, Price = 100, Side = OrderSide.Buy, Size = 10, Symbol = "ABC" };

            var result = Order.CreateFromMessage(message);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.OrderId, Is.EqualTo(message.OrderId));
                Assert.That(result.Price, Is.EqualTo(message.Price));
                Assert.That(result.Side, Is.EqualTo(message.Side));
                Assert.That(result.Size, Is.EqualTo(message.Size));
                Assert.That(result.Symbol, Is.EqualTo(message.Symbol));
            });
        }

        [Test]
        public void CreateFromMessage_AddedMessageNull_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Order.CreateFromMessage((OrderMessageAdded)null!));
        }

        [Test]
        public void CreateFromMessage_UpdatedMessage_ShouldReturnOrder()
        {
            var message = new OrderMessageUpdated { OrderId = 1, Price = 100, Side = OrderSide.Buy, Size = 10, Symbol = "ABC" };

            var result = Order.CreateFromMessage(message);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.OrderId, Is.EqualTo(message.OrderId));
                Assert.That(result.Price, Is.EqualTo(message.Price));
                Assert.That(result.Side, Is.EqualTo(message.Side));
                Assert.That(result.Size, Is.EqualTo(message.Size));
                Assert.That(result.Symbol, Is.EqualTo(message.Symbol));
            });
        }

        [Test]
        public void CreateFromMessage_UpdatedMessageNull_ShouldThrowArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Order.CreateFromMessage((OrderMessageUpdated)null!));
        }
    }
}
