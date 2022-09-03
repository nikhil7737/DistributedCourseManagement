namespace CourseDDBStreamConsumer.Model;

public class CourseEventPascal
{
    public string CourseId { get; set; }
    public int SequenceNo { get; set; }

}

public class CourseEventCamel
{
    public string courseId { get; set; }
    public int sequenceNo { get; set; }

}