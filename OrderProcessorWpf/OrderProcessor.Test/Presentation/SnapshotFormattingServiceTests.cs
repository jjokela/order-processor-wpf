using OrderProcessor.Domain.Entities;
using OrderProcessor.Domain.Enums;
using OrderProcessor.Presentation.Services;

namespace OrderProcessor.Test.Presentation
{
    public class SnapshotFormattingServiceTests
    {
        [Test]
        public void FormatSnapshot_WhenCalled_FormatsSnapshotCorrectly()
        {
            var bidBook = new List<(ulong Volume, int Price)>
            {
                (100, 250),
                (150, 200)
            };

            var askBook = new List<(ulong Volume, int Price)>
            {
                (200, 350),
                (250, 300)
            };

            var snapshot = new Snapshot
            {
                SequenceNumber = 1,
                Symbol = "ABC",
            };
            snapshot.UpdateBookEntries(bidBook, OrderSide.Buy);
            snapshot.UpdateBookEntries(askBook, OrderSide.Sell);

            var result = SnapshotFormattingService.FormatSnapshot(snapshot);

            var expected = "1, ABC, [(250, 100), (200, 150)], [(350, 200), (300, 250)]";

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void FormatSnapshot_WhenNoEntries_FormatsSnapshotCorrectly()
        {
            var snapshot = new Snapshot
            {
                SequenceNumber = 1,
                Symbol = "ABC",
            };

            var result = SnapshotFormattingService.FormatSnapshot(snapshot);

            var expected = "1, ABC, [], []";

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
