using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Entities.MessageHandlers;
using OrderProcessor.Domain.Entities.Messages;
using OrderProcessor.Domain.Interfaces;

namespace OrderProcessor.Domain.Services
{
    public class OrderEngine
    {
        public Dictionary<string, OrderBook> OrderBooks { get; } = new();
        private readonly IOrderMessageRepository _messageRepository;
        private readonly OrderMessageDispatcher _messageDispatcher;

        public OrderEngine(IOrderMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;

            var orderMessageHandlers = InitializeOrderMessageHandlers();

            _messageDispatcher = new OrderMessageDispatcher(orderMessageHandlers);
        }

        private Dictionary<Type, OrderMessageHandlerBase> InitializeOrderMessageHandlers()
        {
            return new Dictionary<Type, OrderMessageHandlerBase>
            {
                { typeof(OrderMessageAdded), new OrderAddedMessageHandler() },
                { typeof(OrderMessageExecuted), new OrderExecutedMessageHandler() },
                { typeof(OrderMessageDeleted), new OrderDeletedMessageHandler() },
                { typeof(OrderMessageUpdated), new OrderUpdatedMessageHandler() }
            };
        }

        public IEnumerable<Snapshot?> ProcessMessages(string filePath, uint depth)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));
            }

            var orderMessages = _messageRepository.GetOrderMessages(filePath);

            if (orderMessages == null)
            {
                throw new InvalidOperationException("Order messages cannot be null.");
            }

            foreach (var orderMessage in orderMessages)
            {
                if (!OrderBooks.TryGetValue(orderMessage.Symbol, out var orderBook))
                {
                    orderBook = new OrderBook(orderMessage.Symbol, depth);
                    OrderBooks.Add(orderMessage.Symbol, orderBook);
                }

                yield return _messageDispatcher.Dispatch(orderMessage, orderBook);
            }
        }
    }
}
