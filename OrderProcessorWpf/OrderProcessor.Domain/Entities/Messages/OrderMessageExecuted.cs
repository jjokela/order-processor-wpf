using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Domain.Entities.Messages
{
    public class OrderMessageExecuted : OrderMessageBase
    {
        public ulong TradedQuantity { get; set; }
    }
}