using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CourseProjector;

public class CourseProjector
{
    private readonly IDynamoDBContext _dbContext;
    public CourseProjector()
    {
        var client = new AmazonDynamoDBClient();
        _dbContext = new DynamoDBContext(client);
    }
    public async Task Project(EBEvent ebEvent, ILambdaContext context)
    {
        string outboxId = ebEvent?.detail?.OutboxId;
        if (await IsEventProcessed(outboxId))
        {
            return;
        }
        Console.WriteLine(JsonSerializer.Serialize(ebEvent));
    }

    private async Task<bool> IsEventProcessed(string outboxId)
    {
        var response = await _dbContext.LoadAsync<ProcessedOutboxEvent>(outboxId);
        return response != null;
    }
}