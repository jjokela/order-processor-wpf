using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Domain.Entities.Messages
{
    public class OrderMessageUpdated : OrderMessageBase
    {
        public ulong Size { get; set; }
        public int Price { get; set; }
    }
}