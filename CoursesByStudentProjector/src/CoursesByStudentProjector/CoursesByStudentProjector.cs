using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Common;
using Common.EBModels;
using Common.Events;
using Common.ExtensionMethods;
using Common.ReadModels;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CoursesByStudentProjector;

public class CoursesByStudentProjector
{
    private const int _snapshotInterval = 3;
    private readonly IDynamoDBContext _dbContext;
    public CoursesByStudentProjector()
    {
        var client = new AmazonDynamoDBClient();
        _dbContext = new DynamoDBContext(client);
    }
    public async Task Project(EBEvent ebEvent, ILambdaContext context)
    {
        try
        {
            string outboxId = ebEvent?.detail?.OutboxId;
            string aggregateId = ebEvent.detail.AggregateId;
            if (outboxId.IsNullOrEmpty())
            {
                return;
            }
            if (await IsEventProcessed(outboxId))
            {
                return;
            }
            Snapshot snapshot = await GetLatestSnapshot(aggregateId);
            int lastSequenceNo = snapshot?.LastSequenceNo ?? 0;
            CourseByStudent courseByStudent = snapshot == null ? new() : JsonSerializer.Deserialize<CourseByStudent>(snapshot.SerializedState);
            List<EnrollmentEvent> eventsToApply = await GetEventsToApply<EnrollmentEvent>(aggregateId, lastSequenceNo + 1);
            eventsToApply.ForEach(e => courseByStudent.ApplyEvent(e));

            //Transaction
            await PersistReadModel(courseByStudent);
            await MarkEventProcessed(outboxId);

            await HandleSnapshotPersistence(courseByStudent, eventsToApply.Last().SequenceNo);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

    }

    //repo methods
    private async Task HandleSnapshotPersistence(CourseByStudent courseByInstructor, int sequenceNo)
    {
        if (sequenceNo % _snapshotInterval == 0)
        {
            await _dbContext.SaveAsync<Snapshot>(new Snapshot
            {
                AggregateId = $"{courseByInstructor.StudentId}-{courseByInstructor.CourseId}",
                LastSequenceNo = sequenceNo,
                SerializedState = JsonSerializer.Serialize(courseByInstructor)
            });
        }
    }

    private async Task MarkEventProcessed(string outboxId)
    {
        await _dbContext.SaveAsync<ProcessedOutboxEvent>(new ProcessedOutboxEvent(outboxId));
    }

    private async Task PersistReadModel(CourseByStudent finalState)
    {
        await _dbContext.SaveAsync<CourseByStudent>(finalState);
    }

    private async Task<bool> IsEventProcessed(string outboxId)
    {
        var response = await _dbContext.LoadAsync<ProcessedOutboxEvent>(outboxId);
        return response != null;
    }

    private async Task<List<T>> GetEventsToApply<T>(string aggregateId, int startSequenceNo)
    {
        var searchResult = _dbContext.QueryAsync<T>
        (
            aggregateId,
            QueryOperator.GreaterThanOrEqual,
            new List<string> { startSequenceNo.ToString() }
        );
        var events = await searchResult.GetRemainingAsync();
        return events;
    }

    private async Task<Snapshot> GetLatestSnapshot(string aggregateId)
    {
        return await _dbContext.LoadAsync<Snapshot>(aggregateId);
    }
}
