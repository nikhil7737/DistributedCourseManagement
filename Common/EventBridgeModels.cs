using Common;

namespace Common.EBModels;

public class EBEvent
{
    public Outbox detail { get; set; }

}

public class ProcessedOutboxEvent
{
    public string OutboxId { get; set; }
    public ProcessedOutboxEvent(string outboxId)
    {
        OutboxId = outboxId;
    }
}