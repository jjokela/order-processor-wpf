namespace OrderProcessor.Domain.Enums
{
    public enum MessageType : byte
    {
        OrderAdded = (byte)'A',
        OrderUpdated = (byte)'U',
        OrderDeleted = (byte)'D',
        OrderExecuted = (byte)'E',
    }
}