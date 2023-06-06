using System.Text;
using OrderProcessor.Domain.Entities.Messages;
using OrderProcessor.Domain.Enums;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Data.Factory
{
    public class OrderMessageFactory
    {
        public static OrderMessageBase GetOrderMessage(OrderMessageHeader orderMessageHeader, byte[] messageData, MessageType messageType)
        {
            return messageType switch
            {
                MessageType.OrderAdded => CreateOrderMessage<OrderMessageAdded>(
                    orderMessageHeader.SequenceNumber, messageData),
                MessageType.OrderUpdated => CreateOrderMessage<OrderMessageUpdated>(
                    orderMessageHeader.SequenceNumber, messageData),
                MessageType.OrderDeleted => CreateOrderMessage<OrderMessageDeleted>(
                    orderMessageHeader.SequenceNumber, messageData),
                MessageType.OrderExecuted => CreateOrderMessage<OrderMessageExecuted>(
                    orderMessageHeader.SequenceNumber, messageData),
                _ => throw new ArgumentOutOfRangeException(nameof(messageType))
            };
        }

        public static T CreateOrderMessage<T>(uint sequenceNumber, byte[] messageData) where T : OrderMessageBase, new()
        {
            using var memoryStream = new MemoryStream(messageData);
            using var binaryReader = new BinaryReader(memoryStream);

            var order = new T
            {
                SequenceNumber = sequenceNumber
            };
            _ = binaryReader.ReadChar(); // discard messageType char since we already know its type
            order.Symbol = Encoding.ASCII.GetString(binaryReader.ReadBytes(3)).TrimEnd();
            order.OrderId = binaryReader.ReadUInt64();
            order.Side = (OrderSide)binaryReader.ReadChar();

            binaryReader.ReadBytes(3); // Skip reserved bytes

            switch (order)
            {
                case OrderMessageAdded orderAdded:
                    orderAdded.Size = binaryReader.ReadUInt64();
                    orderAdded.Price = binaryReader.ReadInt32();
                    binaryReader.ReadBytes(4); // Skip reserved bytes
                    break;
                case OrderMessageUpdated orderUpdated:
                    orderUpdated.Size = binaryReader.ReadUInt64();
                    orderUpdated.Price = binaryReader.ReadInt32();
                    binaryReader.ReadBytes(4); // Skip reserved bytes
                    break;
                case OrderMessageDeleted:
                    break;
                case OrderMessageExecuted orderExecuted:
                    orderExecuted.TradedQuantity = binaryReader.ReadUInt64();
                    break;
                default:
                    throw new NotSupportedException($"Order type {typeof(T)} not supported");
            }

            return order;
        }
    }
}