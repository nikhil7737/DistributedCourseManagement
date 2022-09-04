using Amazon.DynamoDBv2.DataModel;
using Common.Enums;
using Common.Entity;
namespace Common.Events
{
    [DynamoDBTable("CourseEvents", lowerCamelCaseProperties: true)]
    public class CourseEvent
    {
        [DynamoDBHashKey]
        public string CourseId { get; set; }
        [DynamoDBRangeKey]
        public int SequenceNo { get; set; }
        public CourseEventEnum Type {get; set;}
        public Course Course { get; set; }
    }
}