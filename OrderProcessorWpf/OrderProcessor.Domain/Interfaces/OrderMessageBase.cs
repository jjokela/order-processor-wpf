using OrderProcessor.Domain.Enums;

namespace OrderProcessor.Domain.Interfaces
{
    public abstract class OrderMessageBase
    {
        public uint SequenceNumber { get; init; }
        public string Symbol { get; set; } = string.Empty;
        public ulong OrderId { get; set; }
        public OrderSide Side { get; set; }
    }
}