using CommandService.BL.Interfaces;
using Common.Enums;
using CommandService.Models.Commands;
using Common.Events;
using CommandService.Repository.Interfaces;

namespace CommandService.BL
{
    public class CourseBL : ICourseBL
    {
        private readonly ICourseRepo _repo;
        public CourseBL(ICourseRepo repo)
        {
            _repo = repo;
        }

        public async Task Execute(CreateCourseCommand command)
        {
            var courseCreatedEvent = new CourseEvent
            {
                CourseId = Guid.NewGuid().ToString(),
                SequenceNo = 1,
                Type = CourseEventEnum.Created,
                Course = new()
                {
                    Name = command.Name,
                    Description = command.Description,
                    InstructorId = command.InstructorId
                }
            };
            await _repo.PesistCourseEvent(courseCreatedEvent);
        }
        public async Task Execute(DeleteCourseCommand command)
        {
            var courseDeletedEvent = new CourseEvent
            {
                CourseId = Guid.NewGuid().ToString(),
                SequenceNo = await _repo.GetLastSequenceNo(command.CourseId) + 1,
                Type = CourseEventEnum.Deleted,
            };
            await _repo.PesistCourseEvent(courseDeletedEvent);
        }
    }
}