using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Lambda.Core;
using Common;
using Common.Events;
using Common.ExtensionMethods;
using Common.ReadModels;
using Common.EBModels;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CoursesByInstructorProjector;

public class CoursesByInstructorProjector
{
    private const int _snapshotInterval = 3;
    private readonly IDynamoDBContext _dbContext;
    public CoursesByInstructorProjector()
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
            CourseByInstructor courseByInstructor = snapshot == null ? new() : JsonSerializer.Deserialize<CourseByInstructor>(snapshot.SerializedState);
            List<CourseEvent> eventsToApply = await GetEventsToApply<CourseEvent>(aggregateId, lastSequenceNo + 1);
            eventsToApply.ForEach(e => courseByInstructor.ApplyEvent(e));

            //Transaction
            await PersistReadModel(courseByInstructor);
            await MarkEventProcessed(outboxId);

            await HandleSnapshotPersistence(courseByInstructor, eventsToApply.Last().SequenceNo);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

    }

    //repo methods
    private async Task HandleSnapshotPersistence(CourseByInstructor courseByInstructor, int sequenceNo)
    {
        if (sequenceNo % _snapshotInterval == 0)
        {
            await _dbContext.SaveAsync<Snapshot>(new Snapshot
            {
                AggregateId = courseByInstructor.CourseId,
                LastSequenceNo = sequenceNo,
                SerializedState = JsonSerializer.Serialize(courseByInstructor)
            });
        }
    }

    private async Task MarkEventProcessed(string outboxId)
    {
        await _dbContext.SaveAsync<ProcessedOutboxEvent>(new ProcessedOutboxEvent
        {
            OutboxId = outboxId
        });
    }

    private async Task PersistReadModel(CourseByInstructor finalState)
    {
        if (finalState.IsDeleted)
        {
            await _dbContext.DeleteAsync<CourseByInstructor>(finalState);
        }
        else
        {
            await _dbContext.SaveAsync<CourseByInstructor>(finalState);
        }
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
            new List<object> { startSequenceNo }
        );
        var events = await searchResult.GetRemainingAsync();
        return events;
    }

    private async Task<Snapshot> GetLatestSnapshot(string aggregateId)
    {
        return await _dbContext.LoadAsync<Snapshot>(aggregateId);
    }
}
