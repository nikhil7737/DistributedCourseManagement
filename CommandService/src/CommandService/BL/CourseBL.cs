using CommandService.BL.Interfaces;
using CommandService.Enums;
using CommandService.Models.Commands;
using CommandService.Models.Events;
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
            await _repo.PesistCourseCreatedEvent(courseCreatedEvent);
            // await _repo.PesistCourseCreatedEvent(courseCreatedEvent);
        }
    }
}