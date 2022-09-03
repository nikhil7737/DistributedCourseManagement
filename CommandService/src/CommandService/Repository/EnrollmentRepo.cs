using Amazon.DynamoDBv2.DataModel;
using CommandService.Repository.Interfaces;
using CommandService.Models.Events;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Common.Outbox;

namespace CommandService.Repository;
public class EnrollmentRepo : IEnrollmentRepo
{
    private readonly IDynamoDBContext _dbContext;
    public EnrollmentRepo(IDynamoDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task PesistEnrollmentEvent(EnrollmentEvent enrollmentEvent)
    {
        try
        {
            var outbox = new EnrollmentOutbox
            {
                AggregateId = enrollmentEvent.AggregateId,
                SequenceNo = enrollmentEvent.SequenceNo
            };
            await _dbContext.SaveAsync<EnrollmentEvent>(enrollmentEvent);
            await _dbContext.SaveAsync<EnrollmentOutbox>(outbox);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task<int> GetLastSequenceNo(string aggregateId)
    {
        try
        {
            var queryConfig = new QueryOperationConfig
            {
                Limit = 1,
                Filter = new QueryFilter("aggregateId", QueryOperator.Equal, new List<AttributeValue>() { new AttributeValue(aggregateId) }),
                BackwardSearch = false,
                Select = SelectValues.SpecificAttributes,
                AttributesToGet = new List<string> { "sequenceNo" },
            };
            var response = _dbContext.FromQueryAsync<EnrollmentEvent>(queryConfig);
            var events = await response.GetNextSetAsync();
            return events.Count == 0 ? 0 : events.First().SequenceNo;
        }
        catch (Exception e)
        {
            return 0;
        }
    }
}