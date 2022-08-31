using CommandService.Models.Events;

namespace CommandService.Repository.Interfaces
{
    public interface ICourseRepo
    {
        Task PesistCourseEvent(CourseEvent courseCreatedEvent);
        Task<int> GetLastSequenceNo(string courseId);
    }
}