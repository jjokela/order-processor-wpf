using OrderProcessor.Data.Factory;
using OrderProcessor.Domain.Enums;
using System.Text;
using OrderProcessor.Domain.Entities.Messages;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Test.Data
{
    public class OrderMessageFactoryTests
    {
        [Test]
        public void GetOrderMessage_OrderAdded_ReturnsCorrectOrderMessage()
        {
            var orderMessageHeader = new OrderMessageHeader { SequenceNumber = 1 };
            var messageData = CreateOrderAddedMessageData();
            var messageType = MessageType.OrderAdded;

            var result = OrderMessageFactory.GetOrderMessage(orderMessageHeader, messageData, messageType);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<OrderMessageAdded>());
                var orderAdded = (OrderMessageAdded)result;
                Assert.That(orderAdded.SequenceNumber, Is.EqualTo(1));
                Assert.That(orderAdded.Symbol, Is.EqualTo("AAP"));
                Assert.That(orderAdded.OrderId, Is.EqualTo(123456789));
                Assert.That(orderAdded.Side, Is.EqualTo(OrderSide.Buy));
                Assert.That(orderAdded.Size, Is.EqualTo(100));
                Assert.That(orderAdded.Price, Is.EqualTo(200));
            });
        }

        [Test]
        public void GetOrderMessage_OrderUpdated_ReturnsCorrectOrderMessage()
        {
            var orderMessageHeader = new OrderMessageHeader { SequenceNumber = 2 };
            var messageData = CreateOrderUpdatedMessageData();
            var messageType = MessageType.OrderUpdated;

            var result = OrderMessageFactory.GetOrderMessage(orderMessageHeader, messageData, messageType);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<OrderMessageUpdated>());
                var orderUpdated = (OrderMessageUpdated)result;
                Assert.That(orderUpdated.SequenceNumber, Is.EqualTo(2));
                Assert.That(orderUpdated.Symbol, Is.EqualTo("AAP"));
                Assert.That(orderUpdated.OrderId, Is.EqualTo(123456789));
                Assert.That(orderUpdated.Side, Is.EqualTo(OrderSide.Buy));
                Assert.That(orderUpdated.Size, Is.EqualTo(150));
                Assert.That(orderUpdated.Price, Is.EqualTo(250));
            });
        }

        [Test]
        public void GetOrderMessage_OrderDeleted_ReturnsCorrectOrderMessage()
        {
            var orderMessageHeader = new OrderMessageHeader { SequenceNumber = 3 };
            var messageData = CreateOrderDeletedMessageData();
            var messageType = MessageType.OrderDeleted;

            var result = OrderMessageFactory.GetOrderMessage(orderMessageHeader, messageData, messageType);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<OrderMessageDeleted>());
                var orderDeleted = (OrderMessageDeleted)result;
                Assert.That(orderDeleted.SequenceNumber, Is.EqualTo(3));
                Assert.That(orderDeleted.Symbol, Is.EqualTo("AAP"));
                Assert.That(orderDeleted.OrderId, Is.EqualTo(123456789));
                Assert.That(orderDeleted.Side, Is.EqualTo(OrderSide.Buy));
            });
        }

        [Test]
        public void GetOrderMessage_OrderExecuted_ReturnsCorrectOrderMessage()
        {
            var orderMessageHeader = new OrderMessageHeader { SequenceNumber = 4 };
            var messageData = CreateOrderExecutedMessageData();
            var messageType = MessageType.OrderExecuted;

            var result = OrderMessageFactory.GetOrderMessage(orderMessageHeader, messageData, messageType);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<OrderMessageExecuted>());
                var orderExecuted = (OrderMessageExecuted)result;
                Assert.That(orderExecuted.SequenceNumber, Is.EqualTo(4));
                Assert.That(orderExecuted.Symbol, Is.EqualTo("AAP"));
                Assert.That(orderExecuted.OrderId, Is.EqualTo(123456789));
                Assert.That(orderExecuted.Side, Is.EqualTo(OrderSide.Buy));
                Assert.That(orderExecuted.TradedQuantity, Is.EqualTo(50));
            });
        }

        [Test]
        public void GetOrderMessage_InvalidMessageType_ThrowsArgumentOutOfRangeException()
        {
            var orderMessageHeader = new OrderMessageHeader { SequenceNumber = 5 };
            var messageData = CreateInvalidMessageData();
            var messageType = (MessageType)99; // Invalid MessageType

            Assert.Throws<ArgumentOutOfRangeException>(() => OrderMessageFactory.GetOrderMessage(orderMessageHeader, messageData, messageType));
        }

        [Test]
        public void CreateOrderMessage_InvalidOrderType_ThrowsNotSupportedException()
        {
            uint sequenceNumber = 6;
            var messageData = CreateInvalidMessageData();

            Assert.Throws<NotSupportedException>(() => OrderMessageFactory.CreateOrderMessage<InvalidOrderMessage>(sequenceNumber, messageData));
        }

        private byte[] CreateInvalidMessageData()
        {
            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write('X');
            binaryWriter.Write(Encoding.ASCII.GetBytes("AAP"));
            binaryWriter.Write((ulong)123456789);
            binaryWriter.Write('B');
            binaryWriter.Write(new byte[3]);

            return memoryStream.ToArray();
        }

        public class InvalidOrderMessage : OrderMessageBase { }

        private byte[] CreateOrderDeletedMessageData()
        {
            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write('D');
            binaryWriter.Write(Encoding.ASCII.GetBytes("AAP"));
            binaryWriter.Write((ulong)123456789);
            binaryWriter.Write('B');
            binaryWriter.Write(new byte[3]);

            return memoryStream.ToArray();
        }

        private byte[] CreateOrderExecutedMessageData()
        {
            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write('E');
            binaryWriter.Write(Encoding.ASCII.GetBytes("AAP"));
            binaryWriter.Write((ulong)123456789);
            binaryWriter.Write('B');
            binaryWriter.Write(new byte[3]);
            binaryWriter.Write((ulong)50);

            return memoryStream.ToArray();
        }

        private byte[] CreateOrderAddedMessageData()
        {
            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write('A');
            binaryWriter.Write(Encoding.ASCII.GetBytes("AAP"));
            binaryWriter.Write((ulong)123456789);
            binaryWriter.Write('B');
            binaryWriter.Write(new byte[3]);
            binaryWriter.Write((ulong)100);
            binaryWriter.Write((int)200);
            binaryWriter.Write(new byte[4]);

            return memoryStream.ToArray();
        }

        private byte[] CreateOrderUpdatedMessageData()
        {
            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write('U');
            binaryWriter.Write(Encoding.ASCII.GetBytes("AAP"));
            binaryWriter.Write((ulong)123456789);
            binaryWriter.Write('B');
            binaryWriter.Write(new byte[3]);
            binaryWriter.Write((ulong)150);
            binaryWriter.Write((int)250);
            binaryWriter.Write(new byte[4]);

            return memoryStream.ToArray();
        }
    }
}

