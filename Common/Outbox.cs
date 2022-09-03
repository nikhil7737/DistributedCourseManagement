
namespace Common;

public class Outbox
{
    public string OutboxId { get; set; }
    public string AggregateId { get; set; }
    public EventType EventType { get; set; }
}
