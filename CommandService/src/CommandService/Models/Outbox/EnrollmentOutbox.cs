using Amazon.DynamoDBv2.DataModel;
using CommandService.Enums;

namespace CommandService.Models.Outbox
{
    [DynamoDBTable("EnrollmentOutbox", lowerCamelCaseProperties: true)]
    public class EnrollmentOutbox
    {
        public string AggregateId { get; set; }
        public int SequenceNo { get; set; }
    }
}