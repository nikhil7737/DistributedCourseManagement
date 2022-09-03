using System.Net;
using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Amazon.EventBridge;
using Amazon.EventBridge.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Common;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CourseDDBStreamConsumer;

public class Function
{
    private const string _courseEventBus = "CourseEvents";
    private const string _enrollmentEventBus = "EnrollmentEvents";
    private readonly IDynamoDBContext _dbContext;
    public Function()
    {
        var client = new AmazonDynamoDBClient();
        _dbContext = new DynamoDBContext(client);
    }
    public async Task FunctionHandler(DynamoDBEvent dbEvent, ILambdaContext context)
    {
        try
        {
            var record = dbEvent.Records.First();
            var outboxEvent = GetOutboxEvent(record.Dynamodb.Keys);
            if (await SendEventToEventBridge(outboxEvent))
            {
                Console.WriteLine("message sent to event bridge");
                // await _dbContext.DeleteAsync<Outbox>(outboxEvent);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private Outbox GetOutboxEvent(Dictionary<string, AttributeValue> record)
    {
        Document doc = Document.FromAttributeMap(record);
        return _dbContext.FromDocument<Outbox>(doc);
    }

    private async Task<bool> SendEventToEventBridge(Outbox outbox)
    {
        var eventBridgeClient = new AmazonEventBridgeClient();
        var response = await eventBridgeClient.PutEventsAsync(new PutEventsRequest
        {
            Entries = new List<PutEventsRequestEntry> {
                new ()
                {
                    EventBusName = GetEventBus(outbox.EventType),
                    Detail = JsonSerializer.Serialize(outbox)
                }
            }
        });
        return response.HttpStatusCode == HttpStatusCode.OK;
    }
    private static string GetEventBus(EventType eventType)
    {
        switch (eventType)
        {
            case EventType.Course:
                return _courseEventBus;
            case EventType.Enrollment:
                return _enrollmentEventBus;
            default:
                return string.Empty;
        }
    }
}
