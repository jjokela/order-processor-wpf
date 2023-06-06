using OrderProcessor.Domain.Comparers;
using OrderProcessor.Domain.Entities;

namespace OrderProcessor.Test.Domain.Comparers
{
    public class BuyPriceLevelComparerTests
    {
        private readonly BuyPriceLevelComparer _comparer = new();

        [Test]
        public void Compare_BothNull_ReturnsZero()
        {
            var result = _comparer.Compare(null, null);

            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public void Compare_XNull_ReturnsMinusOne()
        {
            var y = new PriceLevel(1);

            var result = _comparer.Compare(null, y);

            Assert.That(result, Is.EqualTo(-1));

        }

        [Test]
        public void Compare_YNull_ReturnsOne()
        {
            var x = new PriceLevel(1);

            var result = _comparer.Compare(x, null);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void Compare_YGreaterThanX_ReturnsComparisonResult()
        {
            var x = new PriceLevel(1);
            var y = new PriceLevel(2);

            var result = _comparer.Compare(x, y);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void Compare_XGreaterThanY_ReturnsComparisonResult()
        {
            var x = new PriceLevel(2);
            var y = new PriceLevel(1);

            var result = _comparer.Compare(x, y);

            Assert.That(result, Is.EqualTo(-1));
        }
    }

}
