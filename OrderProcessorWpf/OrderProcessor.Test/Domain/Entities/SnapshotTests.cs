using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Enums;

namespace OrderProcessor.Test.Domain.Entities
{
    public class SnapshotTests
    {
        [Test]
        public void PriceInVisibleRange_WhenBuySideAndPriceInVisibleRange_ReturnsTrue()
        {
            var snapshot = new Snapshot { Depth = 2 };
            snapshot.UpdateBookEntries(new List<(ulong, int)> { (5, 20), (5, 10) }, OrderSide.Buy);
            var prices = new[] { 15, 30 };

            var result = snapshot.PriceInVisibleRange(prices, OrderSide.Buy);

            Assert.That(result, Is.True);
        }

        [Test]
        public void PriceInVisibleRange_WhenSellSideAndPriceInVisibleRange_ReturnsTrue()
        {
            var snapshot = new Snapshot { Depth = 2 };
            snapshot.UpdateBookEntries(new List<(ulong, int)> { (5, 10), (5, 20) }, OrderSide.Sell);
            var prices = new[] { 15, 5 };

            var result = snapshot.PriceInVisibleRange(prices, OrderSide.Sell);

            Assert.That(result, Is.True);
        }

        [Test]
        public void PriceInVisibleRange_WhenBuySideAndNegativePrice_ReturnsTrue()
        {
            var snapshot = new Snapshot { Depth = 2 };
            snapshot.UpdateBookEntries(new List<(ulong, int)> { (5, 20), (5, -10) }, OrderSide.Buy);

            var prices = new[] { 1-9 };

            var result = snapshot.PriceInVisibleRange(prices, OrderSide.Buy);

            Assert.That(result, Is.True);
        }

        [Test]
        public void PriceInVisibleRange_WhenSellSideAndNegativePrice_ReturnsTrue()
        {
            var snapshot = new Snapshot { Depth = 2 };

            snapshot.UpdateBookEntries(new List<(ulong, int)> { (5, -20), (5, 10) }, OrderSide.Sell);

            var prices = new[] { -5 };

            var result = snapshot.PriceInVisibleRange(prices, OrderSide.Sell);

            Assert.That(result, Is.True);
        }

        [Test]
        public void UpdateBookEntries_WhenBuySide_UpdatesBidBook()
        {
            var snapshot = new Snapshot();
            var bookEntries = new List<(ulong, int)> { (5, 20), (5, 10) };

            snapshot.UpdateBookEntries(bookEntries, OrderSide.Buy);

            Assert.That(snapshot.BidBook, Is.EqualTo(bookEntries));
        }

        [Test]
        public void UpdateBookEntries_WhenSellSide_UpdatesAskBook()
        {
            var snapshot = new Snapshot();
            var bookEntries = new List<(ulong, int)> { (5, 10), (5, 20) };

            snapshot.UpdateBookEntries(bookEntries, OrderSide.Sell);

            Assert.That(snapshot.AskBook, Is.EqualTo(bookEntries));
        }

        [Test]
        public void UpdateBookEntries_WhenInvalidSide_ThrowsArgumentOutOfRangeException()
        {
            var snapshot = new Snapshot();
            var bookEntries = new List<(ulong, int)> { (5, 10), (5, 20) };

            Assert.That(() => snapshot.UpdateBookEntries(bookEntries, (OrderSide)99), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void BidBookLowestPrice_WhenBidBookIsNotEmpty_ReturnsLowestPrice()
        {
            var snapshot = new Snapshot();
            snapshot.UpdateBookEntries(new List<(ulong, int)> { (5, 20), (5, 10) }, OrderSide.Buy);

            var result = snapshot.BidBookLowestVisiblePrice;

            Assert.That(result, Is.EqualTo(10));
        }

        [Test]
        public void AskBookHighestPrice_WhenAskBookIsNotEmpty_ReturnsHighestPrice()
        {
            var snapshot = new Snapshot();
            snapshot.UpdateBookEntries(new List<(ulong, int)> { (5, 10), (5, 20) }, OrderSide.Sell);

            var result = snapshot.AskBookHighestVisiblePrice;

            Assert.That(result, Is.EqualTo(20));
        }

        [Test]
        public void Snapshot_WhenInitialized_HasExpectedDefaultValues()
        {
            var snapshot = new Snapshot { Depth = 2, SequenceNumber = 3, Symbol = "ABC" };

            Assert.Multiple(() =>
            {
                Assert.That(snapshot.Depth, Is.EqualTo(2));
                Assert.That(snapshot.SequenceNumber, Is.EqualTo(3));
                Assert.That(snapshot.Symbol, Is.EqualTo("ABC"));
                Assert.That(snapshot.IsUpdated, Is.False);
            });
        }
    }
}
