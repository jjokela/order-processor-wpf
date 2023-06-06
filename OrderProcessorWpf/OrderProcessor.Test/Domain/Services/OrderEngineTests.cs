using Moq;
using OrderProcessor.Domain.Entities.Messages;
using OrderProcessor.Domain.Interfaces;
using OrderProcessor.Domain.Services;

namespace OrderProcessor.Test.Domain.Services
{
    public class OrderEngineTests
    {
        private Mock<IOrderMessageRepository> _messageRepositoryMock = null!;
        private OrderEngine _orderEngine = null!;

        [SetUp]
        public void Setup()
        {
            _messageRepositoryMock = new Mock<IOrderMessageRepository>();
            _orderEngine = new OrderEngine(_messageRepositoryMock.Object);
        }

        [Test]
        public void ProcessMessages_WhenFilePathIsNull_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _orderEngine.ProcessMessages(null!, 10).ToList());
        }

        [Test]
        public void ProcessMessages_WhenFilePathIsEmpty_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _orderEngine.ProcessMessages(string.Empty, 10).ToList());
        }

        [Test]
        public void ProcessMessages_WhenFilePathIsWhitespace_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _orderEngine.ProcessMessages(" ", 10).ToList());
        }

        [Test]
        public void ProcessMessages_WhenOrderMessagesIsNull_ThrowsInvalidOperationException()
        {
            _messageRepositoryMock.Setup(x => x.GetOrderMessages(It.IsAny<string>())).Returns((IEnumerable<OrderMessageBase>)null!);

            Assert.Throws<InvalidOperationException>(() => _orderEngine.ProcessMessages("validfile", 10).ToList());
        }

        [Test]
        public void ProcessMessages_WhenOrderMessagesAreValid_AddsOrderBookToOrderBooks()
        {
            var orderMessages = new List<OrderMessageBase>
        {
            new OrderMessageAdded { Symbol = "ABC" },
            new OrderMessageAdded { Symbol = "DEF" }
        };

            _messageRepositoryMock.Setup(x => x.GetOrderMessages(It.IsAny<string>())).Returns(orderMessages);

            var snapshots = _orderEngine.ProcessMessages("validfile", 10).ToList();

            Assert.Multiple(() =>
            {
                Assert.That(_orderEngine.OrderBooks.Count, Is.EqualTo(2));
                Assert.That(_orderEngine.OrderBooks.ContainsKey("ABC"), Is.True);
                Assert.That(_orderEngine.OrderBooks.ContainsKey("DEF"), Is.True);
            });
        }
    }
}
