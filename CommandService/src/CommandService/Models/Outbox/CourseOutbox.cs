using Amazon.DynamoDBv2.DataModel;
using CommandService.Enums;

namespace CommandService.Models.Outbox
{
    [DynamoDBTable("CourseOutbox", lowerCamelCaseProperties: true)]
    public class CourseOutbox
    {
        public string CourseId { get; set; }
        public int SequenceNo { get; set; }
    }
}