namespace OrderProcessor.Domain.Interfaces
{
    public interface IOrderMessageRepository
    {
        public IEnumerable<OrderMessageBase> GetOrderMessages(string filePath);
    }
}
