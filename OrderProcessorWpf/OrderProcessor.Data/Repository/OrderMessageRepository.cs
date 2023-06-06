using OrderProcessor.Data.Factory;
using OrderProcessor.Domain.Enums;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Data.Repository
{
    public class OrderMessageRepository : IOrderMessageRepository
    {
        public IEnumerable<OrderMessageBase> GetOrderMessages(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ArgumentException("File doesn't exist");
            }

            if (new FileInfo(filePath).Length == 0)
            {
                throw new ArgumentException("File is empty");
            }

            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var binaryReader = new BinaryReader(fileStream);

            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                var orderHeader = OrderHeaderFactory.CreateOrderHeader(binaryReader);

                var messageData = binaryReader.ReadBytes((int)orderHeader.MessageSize);
                var messageType = (MessageType)messageData[0];


                var order = OrderMessageFactory.GetOrderMessage(orderHeader, messageData, messageType);
                yield return order;
            }
        }
    }
}
