using Moq;
using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Entities.MessageHandlers;
using OrderProcessor.Domain.Entities.Messages;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Test.Domain.MessageHandlers
{
    public class OrderAddedMessageHandlerTests
    {
        private OrderAddedMessageHandler _handler = null!;
        private Mock<IOrderBook> _orderBookMock = null!;

        [SetUp]
        public void Setup()
        {
            _handler = new OrderAddedMessageHandler();
            _orderBookMock = new Mock<IOrderBook>();
        }

        [Test]
        public void HandleOrderMessage_WhenOrderMessageIsNull_ThrowsArgumentNullException()
        {
            Assert.That(() => _handler.HandleOrderMessage(null!, _orderBookMock.Object), Throws.ArgumentNullException);
        }

        [Test]
        public void HandleOrderMessage_WhenOrderBookIsNull_ThrowsArgumentNullException()
        {
            var orderMessage = new OrderMessageAdded();
            Assert.That(() => _handler.HandleOrderMessage(orderMessage, null!), Throws.ArgumentNullException);
        }

        [Test]
        public void HandleOrderMessage_WhenOrderMessageIsNotOrderMessageAdded_ThrowsArgumentException()
        {
            var orderMessage = new OrderMessageUpdated();
            Assert.That(() => _handler.HandleOrderMessage(orderMessage, _orderBookMock.Object), Throws.ArgumentException);
        }

        [Test]
        public void HandleOrderMessage_WhenOrderMessageIsOrderMessageAdded_AddsOrderToOrderBook()
        {
            var orderMessageAdded = new OrderMessageAdded();
            _handler.HandleOrderMessage(orderMessageAdded, _orderBookMock.Object);

            _orderBookMock.Verify(x => x.AddOrder(It.IsAny<Order>(), orderMessageAdded.SequenceNumber), Times.Once);
        }
    }
}
