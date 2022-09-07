using System.Text.Json;
using System.Text.Json.Serialization;
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
    public int UnenrollmentCount { get; set; }
    public string AvgTimeToUnenroll { get; set; }
    [JsonIgnore]
    public DateTime LastEnrollmentDate { get; set; }

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
        LastEnrollmentDate = eventData.Date;
        CurrentStatus = "Enrolled";
    }
    private void ApplyUnenrolledEvent(Enrollment eventData)
    {
        TimeSpan avgTimeToUnenroll = JsonSerializer.Deserialize<TimeSpan>(this.AvgTimeToUnenroll);
        TimeSpan totalTimeInEnrolledState = avgTimeToUnenroll * UnenrollmentCount;
        totalTimeInEnrolledState += eventData.Date - LastEnrollmentDate;
        ++UnenrollmentCount;
        avgTimeToUnenroll = totalTimeInEnrolledState.Divide(UnenrollmentCount);
        this.AvgTimeToUnenroll = JsonSerializer.Serialize(avgTimeToUnenroll);
        CurrentStatus = "Unenrolled";
    }
    private void ApplyFinishedEvent(Enrollment eventData)
    {
        CurrentStatus = "Finished";
    }
}