using OrderProcessor.Data.Repository;
using OrderProcessor.Domain.Interfaces;
using OrderProcessor.Domain.Services;

namespace OrderProcessor.Presentation.Services;

public class OrderProcessingService
{
    private readonly OrderEngine _orderEngine;

    public OrderProcessingService()
    {
        IOrderMessageRepository orderMessageRepository = new OrderMessageRepository();
        _orderEngine = new OrderEngine(orderMessageRepository);
    }

    public void ProcessOrders(string filePath, uint depth)
    {
        var snapshots = _orderEngine.ProcessMessages(filePath, depth);

        foreach (var snapshot in snapshots)
        {
            if (snapshot is not null)
            {
                Console.WriteLine(SnapshotFormattingService.FormatSnapshot(snapshot));
            }
        }
    }
}