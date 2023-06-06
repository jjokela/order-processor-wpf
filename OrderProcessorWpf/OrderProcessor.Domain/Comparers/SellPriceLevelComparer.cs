using OrderProcessor.Domain.Entities;

namespace OrderProcessor.Domain.Comparers
{
    public class SellPriceLevelComparer : IComparer<PriceLevel>
    {
        public int Compare(PriceLevel? x, PriceLevel? y)
        {
            return (x, y) switch
            {
                (null, null) => 0,
                (null, _) => -1,
                (_, null) => 1,
                (_, _) => x.Price.CompareTo(y.Price)
            };
        }
    }
}
