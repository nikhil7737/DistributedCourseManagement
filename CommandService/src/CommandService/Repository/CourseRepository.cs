using Amazon.DynamoDBv2.DataModel;
using CommandService.Repository.Interfaces;
using Amazon.DynamoDBv2;
using Common.Events;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Common;

namespace CommandService.Repository
{
    public class CourseRepo : ICourseRepo
    {
        private readonly IDynamoDBContext _dbContext;
        private readonly IAmazonDynamoDB _dynamoDbClient;
        public CourseRepo(IDynamoDBContext dbContext, IAmazonDynamoDB dynamoDbClient)
        {
            _dbContext = dbContext;
            _dynamoDbClient = dynamoDbClient;
        }
        public async Task PesistCourseEvent(CourseEvent courseEvent)
        {
            try
            {
                var outbox = new Outbox
                {
                    OutboxId = Guid.NewGuid().ToString(),
                    AggregateId = courseEvent.CourseId,
                    EventType = EventType.Course,
                };
                await _dbContext.SaveAsync<CourseEvent>(courseEvent);
                await _dbContext.SaveAsync<Outbox>(outbox);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task<int> GetLastSequenceNo(string courseId)
        {
            try
            {
                var queryConfig = new QueryOperationConfig
                {
                    Limit = 1,
                    Filter = new QueryFilter("courseId", QueryOperator.Equal, new List<AttributeValue>() { new AttributeValue(courseId) }),
                    BackwardSearch = false,
                    Select = SelectValues.SpecificAttributes,
                    AttributesToGet = new List<string> { "sequenceNo" },
                };
                var response = _dbContext.FromQueryAsync<CourseEvent>(queryConfig);
                var a = await response.GetNextSetAsync();
                return a.FirstOrDefault()?.SequenceNo ?? 0;
            }
            catch (Exception e)
            {
                return -1;
            }
        }
    }
}