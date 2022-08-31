using CommandService.Models.Events;

namespace CommandService.Repository.Interfaces
{
    public interface ICourseRepo
    {
        Task PesistCourseCreatedEvent(CourseEvent courseCreatedEvent);
        Task<int> GetLastSequenceNo(string courseId);
    }
}