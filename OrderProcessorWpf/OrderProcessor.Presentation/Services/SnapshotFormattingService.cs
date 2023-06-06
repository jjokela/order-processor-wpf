using OrderProcessor.Domain.Entities;

namespace OrderProcessor.Presentation.Services;

public class SnapshotFormattingService
{
    public static string FormatSnapshot(Snapshot snapshot)
    {
        var bidBookString = string.Join(", ", snapshot.BidBook.Select(x => $"({x.Price}, {x.Volume})"));
        var askBookString = string.Join(", ", snapshot.AskBook.Select(x => $"({x.Price}, {x.Volume})"));

        return $"{snapshot.SequenceNumber}, {snapshot.Symbol}, [{bidBookString}], [{askBookString}]";
    }
}