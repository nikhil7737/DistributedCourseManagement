namespace Common;

public class Snapshot
{
    public string AggregateId { get; set; }
    public int LastSequenceNo { get; set; }
    public string SerializedState { get; set; }
}