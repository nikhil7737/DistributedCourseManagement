using Amazon.DynamoDBv2.DataModel;
using CommandService.Enums;
using CommandService.Models.Entity;

namespace CommandService.Models.Events;

[DynamoDBTable("EnrollmentEvent", lowerCamelCaseProperties: true)]
public class EnrollmentEvent
{
    [DynamoDBHashKey]
    public string AggregateId { get; set; }
    [DynamoDBRangeKey]
    public int SequenceNo { get; set; }
    public EnrollmentType Type { get; set; }
    public Enrollment Enrollment { get; set; }
}