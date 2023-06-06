using Moq;
using OrderProcessor.Domain.Entities.MessageHandlers;
using OrderProcessor.Domain.Entities.Messages;
using OrderProcessor.Domain.Enums;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Test.Domain.MessageHandlers
{
    public class OrderDeletedMessageHandlerTests
    {
        private OrderDeletedMessageHandler _handler = null!;
        private Mock<IOrderBook> _orderBookMock = null!;

        [SetUp]
        public void Setup()
        {
            _handler = new OrderDeletedMessageHandler();
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
            var orderMessage = new OrderMessageDeleted();
            Assert.That(() => _handler.HandleOrderMessage(orderMessage, null!), Throws.ArgumentNullException);
        }

        [Test]
        public void HandleOrderMessage_WhenOrderMessageIsNotOrderMessageDeleted_ThrowsArgumentException()
        {
            var orderMessage = new OrderMessageAdded();
            Assert.That(() => _handler.HandleOrderMessage(orderMessage, _orderBookMock.Object), Throws.ArgumentException);
        }

        [Test]
        public void HandleOrderMessage_WhenOrderMessageIsOrderMessageDeleted_DeletesOrderFromOrderBook()
        {
            var orderMessageDeleted = new OrderMessageDeleted
            {
                OrderId = 1,
                Side = OrderSide.Buy,
                SequenceNumber = 2
            };
            _handler.HandleOrderMessage(orderMessageDeleted, _orderBookMock.Object);

            _orderBookMock.Verify(x => x.DeleteOrder(orderMessageDeleted.OrderId, orderMessageDeleted.Side, orderMessageDeleted.SequenceNumber), Times.Once);
        }
    }
}
