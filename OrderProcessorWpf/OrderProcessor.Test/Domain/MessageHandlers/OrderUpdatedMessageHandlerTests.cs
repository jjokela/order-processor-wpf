using Moq;
using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Entities.MessageHandlers;
using OrderProcessor.Domain.Entities.Messages;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Test.Domain.MessageHandlers
{
    public class OrderUpdatedMessageHandlerTests
    {
        private OrderUpdatedMessageHandler _handler = null!;
        private Mock<IOrderBook> _orderBookMock = null!;

        [SetUp]
        public void Setup()
        {
            _handler = new OrderUpdatedMessageHandler();
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
            var orderMessage = new OrderMessageUpdated();
            Assert.That(() => _handler.HandleOrderMessage(orderMessage, null!), Throws.ArgumentNullException);
        }

        [Test]
        public void HandleOrderMessage_WhenOrderMessageIsNotOrderMessageUpdated_ThrowsArgumentException()
        {
            var orderMessage = new OrderMessageAdded();
            Assert.That(() => _handler.HandleOrderMessage(orderMessage, _orderBookMock.Object), Throws.ArgumentException);
        }

        [Test]
        public void HandleOrderMessage_WhenOrderMessageIsOrderMessageUpdated_UpdatesOrderInOrderBook()
        {
            var orderMessageUpdated = new OrderMessageUpdated { SequenceNumber = 2 };

            _handler.HandleOrderMessage(orderMessageUpdated, _orderBookMock.Object);

            _orderBookMock.Verify(x => x.UpdateOrder(It.IsAny<Order>(), orderMessageUpdated.SequenceNumber), Times.Once);
        }
    }
}
