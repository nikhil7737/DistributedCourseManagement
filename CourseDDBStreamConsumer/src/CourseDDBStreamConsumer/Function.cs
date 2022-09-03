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
using Common.Outbox;
using CourseDDBStreamConsumer.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CourseDDBStreamConsumer;

public class Function
{
    private const string _courseEventBusName = "CourseEvents";
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
            var courseEventCamel = GetCourseEvent(record.Dynamodb.Keys);
            var courseEventPascal = new CourseEventPascal
            {
                CourseId = courseEventCamel.courseId,
                SequenceNo = courseEventCamel.sequenceNo
            };
            if (await SendEventToEventBridge(courseEventPascal))
            {
                await _dbContext.DeleteAsync<CourseOutbox>(courseEventPascal.CourseId, courseEventPascal.SequenceNo);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private CourseEventCamel GetCourseEvent(Dictionary<string, AttributeValue> record)
    {
        Document doc = Document.FromAttributeMap(record);
        return _dbContext.FromDocument<CourseEventCamel>(doc);
    }

    private async Task<bool> SendEventToEventBridge(CourseEventPascal courseEvent)
    {
        var eventBridgeClient = new AmazonEventBridgeClient();
        var response = await eventBridgeClient.PutEventsAsync(new PutEventsRequest
        {
            Entries = new List<PutEventsRequestEntry> {
                new () {
                    EventBusName = _courseEventBusName,
                    Detail = JsonSerializer.Serialize(courseEvent)
                }
            }
        });
        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}
