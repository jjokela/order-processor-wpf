namespace OrderProcessor.Domain.Entities.Messages
{
    public class OrderMessageHeader
    {
        public uint SequenceNumber { get; set; }
        public uint MessageSize { get; set; }
    }
}