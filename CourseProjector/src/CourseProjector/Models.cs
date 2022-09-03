using Common;

namespace CourseProjector;

public class EBEvent
{
    public Outbox detail { get; set; }

}

public class ProcessedOutboxEvent
{
    public string OutboxId { get; set; }
}