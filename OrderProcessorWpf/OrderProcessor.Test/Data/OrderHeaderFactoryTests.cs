using OrderProcessor.Data.Factory;

namespace OrderProcessor.Test.Data
{
    public class OrderHeaderFactoryTests
    {
        [Test]
        public void CreateOrderHeader_CorrectlyInterpretsValues()
        {
            var expectedSequenceNumber = (uint)12345;
            var expectedMessageSize = (uint)67890;

            // Create MemoryStream and BinaryWriter to create in-memory binary data
            using var memoryStream = new MemoryStream();
            using var binaryWriter = new BinaryWriter(memoryStream);

            binaryWriter.Write(expectedSequenceNumber);
            binaryWriter.Write(expectedMessageSize);

            memoryStream.Seek(0, SeekOrigin.Begin);

            using var binaryReader = new BinaryReader(memoryStream);
            var result = OrderHeaderFactory.CreateOrderHeader(binaryReader);

            Assert.Multiple(() =>
            {
                Assert.That(result.SequenceNumber, Is.EqualTo(expectedSequenceNumber));
                Assert.That(result.MessageSize, Is.EqualTo(expectedMessageSize));
            });
        }
    }







}
