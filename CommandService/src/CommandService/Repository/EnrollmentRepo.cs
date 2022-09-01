using Amazon.DynamoDBv2.DataModel;
using CommandService.Repository.Interfaces;
using Amazon.DynamoDBv2;
using CommandService.Models.Events;
using CommandService.Models.Outbox;
using CommandService.Enums;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using CommandService.Models.Commands;

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
                Status = EventStatus.UnProcessed
            };
            await _dbContext.SaveAsync<EnrollmentEvent>(enrollmentEvent);
            await _dbContext.SaveAsync<EnrollmentOutbox>(outbox);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}