using Amazon.DynamoDBv2.DataModel;
using Common.Enums;
using Common.Entity;

namespace Common.Events;

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