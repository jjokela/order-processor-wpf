using OrderProcessor.Domain.Enums;

namespace OrderProcessor.Domain.Entities
{
    public class Snapshot
    {
        public uint Depth { get; init; }
        public uint SequenceNumber { get; set; }
        public string Symbol { get; init; } = string.Empty;
        public List<(ulong Volume, int Price)> BidBook { get; private set; } = new();
        public List<(ulong Volume, int Price)> AskBook { get; private set;  } = new();

        public int BidBookLowestVisiblePrice => BidBook.LastOrDefault().Price;
        public int AskBookHighestVisiblePrice => AskBook.LastOrDefault().Price;

        public bool IsUpdated { get; set; }

        public Snapshot()
        {
            
        }

        public Snapshot(Snapshot snapshot)
        {
            Depth = snapshot.Depth;
            SequenceNumber = snapshot.SequenceNumber;
            Symbol = snapshot.Symbol;
            BidBook = new List<(ulong, int)>(snapshot.BidBook);
            AskBook = new List<(ulong, int)>(snapshot.AskBook);
            IsUpdated = snapshot.IsUpdated;
        }

        public bool PriceInVisibleRange(IEnumerable<int> prices, OrderSide side)
        {
            return side switch
            {
                OrderSide.Buy => prices.Any(p => p >= BidBookLowestVisiblePrice) || BidBook.Count < Depth,
                OrderSide.Sell => prices.Any(p => p <= AskBookHighestVisiblePrice) || AskBook.Count < Depth,
                _ => false
            };
        }

        public void UpdateBookEntries(List<(ulong Volume, int Price)> bookEntries, OrderSide side)
        {
            var bookEntriesList = bookEntries.ToList();
            switch (side)
            {
                case OrderSide.Buy:
                    BidBook = bookEntriesList;
                    break;
                case OrderSide.Sell:
                    AskBook = bookEntriesList;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }
        }
    }
}
