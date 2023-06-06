using Moq;
using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Entities.Messages;
using OrderProcessor.Domain.Interfaces;
using OrderProcessor.Domain.Services;

namespace OrderProcessor.Test.Domain.Services;

public class OrderMessageDispatcherTests
{
    private Mock<OrderMessageHandlerBase> _messageHandlerMock = null!;
    private Dictionary<Type, OrderMessageHandlerBase> _messageHandlers = null!;
    private OrderMessageDispatcher _dispatcher = null!;

    [SetUp]
    public void Setup()
    {
        _messageHandlerMock = new Mock<OrderMessageHandlerBase>();
        _messageHandlers = new Dictionary<Type, OrderMessageHandlerBase>
        {
            { typeof(OrderMessageAdded), _messageHandlerMock.Object },
            { typeof(OrderMessageUpdated), _messageHandlerMock.Object }
        };
        _dispatcher = new OrderMessageDispatcher(_messageHandlers);
    }

    [Test]
    public void Dispatch_WhenOrderIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _dispatcher.Dispatch(null!, new OrderBook("test", 10)));
    }

    [Test]
    public void Dispatch_WhenOrderBookIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _dispatcher.Dispatch(new OrderMessageAdded(), null!));
    }

    [Test]
    public void Dispatch_WhenOrderMessageTypeIsNotSupported_ThrowsNotSupportedException()
    {
        var unsupportedMessage = new OrderMessageDeleted();

        Assert.Throws<NotSupportedException>(() => _dispatcher.Dispatch(unsupportedMessage, new OrderBook("test", 10)));
    }

    [Test]
    public void Dispatch_WhenOrderMessageTypeIsSupported_CallsHandleOrderMessage()
    {
        var supportedMessage = new OrderMessageAdded();
        var orderBook = new OrderBook("test", 10);
        _messageHandlerMock.Setup(x => x.HandleOrderMessage(It.IsAny<OrderMessageBase>(), It.IsAny<OrderBook>()));

        _dispatcher.Dispatch(supportedMessage, orderBook);

        _messageHandlerMock.Verify(x => x.HandleOrderMessage(supportedMessage, orderBook), Times.Once);
    }
}