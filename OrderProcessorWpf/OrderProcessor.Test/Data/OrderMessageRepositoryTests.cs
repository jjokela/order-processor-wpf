using OrderProcessor.Data.Repository;

namespace OrderProcessor.Test.Data
{
    public class OrderMessageRepositoryTests
    {
        private OrderMessageRepository _orderMessageRepository = null!;

        [SetUp]
        public void SetUp()
        {
            _orderMessageRepository = new OrderMessageRepository();
        }

        [Test]
        [Description("Integration test, reads actual file")]
        [Explicit]
        public void GetOrderMessages_ReadsAndProcessesFile()
        {
            var fileName = "input1.stream";

            // Get the path to the output directory
            var outputDirectory = TestContext.CurrentContext.TestDirectory;

            var filePath = Path.Combine(outputDirectory, fileName);

            var results = _orderMessageRepository.GetOrderMessages(filePath);

            Assert.That(results.Count, Is.EqualTo(9));
        }


        [Test]
        public void GetOrderMessages_FileDoesNotExist_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => _orderMessageRepository.GetOrderMessages("non-existent-file-path").ToList());
        }
    }
}
