using Amazon.DynamoDBv2.DataModel;
using Common.Entity;
using Common.Enums;
using Common.Events;

namespace Common.ReadModels;

[DynamoDBTable("CourseByInstructor", lowerCamelCaseProperties: true)]
public class CourseByInstructor
{
    public string InstructorId { get; set; }
    public string CourseId { get; set; }
    public string CourseName { get; set; }
    public int CompletedBy { get; set; }
    public int CurrentlyEnrolled { get; set; }
    public int MaxEnrollment { get; set; }
    public int Fees { get; set; }

    [DynamoDBIgnore]
    public bool IsDeleted { get; set; }


    public void ApplyEvent(CourseEvent courseEvent)
    {
        Course course = courseEvent.Course;
        CourseEventEnum eventType = courseEvent.Type;
        switch (eventType)
        {
            case CourseEventEnum.Created:
                ApplyCourseCreatedEvent(course);
                return;
            case CourseEventEnum.FeesUpdated:
                ApplyFeesUpdatedEvent(course);
                return;
            case CourseEventEnum.NameUpdated:
                ApplyNameUpdatedEvent(course);
                return;
            case CourseEventEnum.MaxEnrollmentCountUpdated:
                ApplyEnrollmentCountUpdatedEvent(course);
                return;
            case CourseEventEnum.Deleted:
                ApplyDeletedEvent();
                return;
        }
    }
    public void ApplyCourseCreatedEvent(Course course)
    {
        InstructorId = course.InstructorId;
        CourseId = course.Id;
        CourseName = course.Name;
        MaxEnrollment = course.MaxEnrollment;
        Fees = course.Fees;
    }
    private void ApplyNameUpdatedEvent(Course course)
    {
        CourseName = course.Name;
    }
    private void ApplyEnrollmentCountUpdatedEvent(Course course)
    {
        MaxEnrollment = course.MaxEnrollment;
    }

    private void ApplyFeesUpdatedEvent(Course course)
    {
        Fees = course.Fees;
    }

    private void ApplyDeletedEvent()
    {
        IsDeleted = true;
    }

}