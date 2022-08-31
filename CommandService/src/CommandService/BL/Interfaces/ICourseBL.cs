using CommandService.Models.Commands;

namespace CommandService.BL.Interfaces
{
    public interface ICourseBL
    {
        Task Execute(CreateCourseCommand command);
    }
}