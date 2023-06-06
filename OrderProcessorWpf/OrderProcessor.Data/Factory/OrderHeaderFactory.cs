using OrderProcessor.Domain.Entities.Messages;

namespace OrderProcessor.Data.Factory
{
    public class OrderHeaderFactory
    {
        public static OrderMessageHeader CreateOrderHeader(BinaryReader binaryReader)
        {
            var orderHeader = new OrderMessageHeader
            {
                SequenceNumber = binaryReader.ReadUInt32(),
                MessageSize = binaryReader.ReadUInt32()
            };

            return orderHeader;
        }
    }
}
