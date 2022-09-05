using Amazon.DynamoDBv2.DataModel;
using Common.Entity;
using Common.Enums;
using Common.Events;

namespace Common.ReadModels;

[DynamoDBTable("CourseByStudent", lowerCamelCaseProperties: true)]
public class CourseByStudent
{
    public string StudentId { get; set; }
    public string CourseId { get; set; }
    public string CourseName { get; set; }
    public string CurrentStatus { get; set; }
    public string FirstEnrollmentDate { get; set; }
    public int UnenrollmentCount { get; set; }
    public int AvgMinutesToUnenroll { get; set; }

    [DynamoDBIgnore]
    private int TotalTimeInEnrolledState { get; set; }
    [DynamoDBIgnore]
    private string LastEnrollmentDate { get; set; }

    public void ApplyEvent(EnrollmentEvent enrollmentEvent)
    {
        EnrollmentType eventType = enrollmentEvent.Type;
        Enrollment eventData = enrollmentEvent.Enrollment;
        switch (eventType)
        {
            case EnrollmentType.Enroll:
                ApplyEnrolledEvent(eventData);
                return;
            case EnrollmentType.Unenroll:
                ApplyUnenrolledEvent(eventData);
                return;
            case EnrollmentType.Finish:
                ApplyFinishedEvent(eventData);
                return;

        }
    }
    private void ApplyEnrolledEvent(Enrollment eventData)
    {
        throw new NotImplementedException();
    }
    private void ApplyUnenrolledEvent(Enrollment eventData)
    {
        throw new NotImplementedException();
    }
    private void ApplyFinishedEvent(Enrollment eventData)
    {
        throw new NotImplementedException();
    }
}