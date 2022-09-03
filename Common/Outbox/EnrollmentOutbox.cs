using Amazon.DynamoDBv2.DataModel;

namespace Common.Outbox
{
    [DynamoDBTable("EnrollmentOutbox", lowerCamelCaseProperties: true)]
    public class EnrollmentOutbox
    {
        public string AggregateId { get; set; }
        public int SequenceNo { get; set; }
    }
}