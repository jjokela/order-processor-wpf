using Moq;
using OrderProcessor.Domain.Entities.MessageHandlers;
using OrderProcessor.Domain.Entities.Messages;
using OrderProcessor.Domain.Enums;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Test.Domain.MessageHandlers
{
    public class OrderExecutedMessageHandlerTests
    {
        private OrderExecutedMessageHandler _handler = null!;
        private Mock<IOrderBook> _orderBookMock = null!;

        [SetUp]
        public void Setup()
        {
            _handler = new OrderExecutedMessageHandler();
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
            var orderMessage = new OrderMessageExecuted();
            Assert.That(() => _handler.HandleOrderMessage(orderMessage, null!), Throws.ArgumentNullException);
        }

        [Test]
        public void HandleOrderMessage_WhenOrderMessageIsNotOrderMessageExecuted_ThrowsArgumentException()
        {
            var orderMessage = new OrderMessageAdded();
            Assert.That(() => _handler.HandleOrderMessage(orderMessage, _orderBookMock.Object), Throws.ArgumentException);
        }

        [Test]
        public void HandleOrderMessage_WhenOrderMessageIsOrderMessageExecuted_ExecutesOrderFromOrderBook()
        {
            var orderMessageExecuted = new OrderMessageExecuted
            {
                OrderId = 1,
                Side = OrderSide.Buy,
                TradedQuantity = 10,
                SequenceNumber = 2
            };
            _handler.HandleOrderMessage(orderMessageExecuted, _orderBookMock.Object);

            _orderBookMock.Verify(x => x.ExecuteOrder(orderMessageExecuted.OrderId, orderMessageExecuted.Side, orderMessageExecuted.TradedQuantity, orderMessageExecuted.SequenceNumber), Times.Once);
        }
    }
}
