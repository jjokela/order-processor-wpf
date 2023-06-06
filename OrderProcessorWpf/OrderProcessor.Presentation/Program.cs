using OrderProcessor.Presentation.Services;

namespace OrderProcessor.Presentation
{
    internal class Program
    {
        private static readonly OrderProcessingService OrderProcessingService = new();

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("The application requires exactly two parameters: [filepath] [depth].");
                return;
            }

            var filePath = args[0];

            if (!uint.TryParse(args[1], out var depth))
            {
                Console.WriteLine("The depth parameter should be a positive integer.");
                return;
            }

            OrderProcessingService.ProcessOrders(filePath, depth);
        }
    }
}