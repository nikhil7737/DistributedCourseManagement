using Amazon.DynamoDBv2.DataModel;

namespace CourseDDBStreamConsumer.Model;

[DynamoDBTable("CourseOutbox", lowerCamelCaseProperties: true)]
public class CourseOutbox
{
    public string CourseId { get; set; }
    public int SequenceNo { get; set; }
}
