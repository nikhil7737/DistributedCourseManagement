using CommandService.Models.Commands;

namespace CommandService.BL.Interfaces
{
    public interface IEnrollmentBL
    {
        Task Execute(EnrollmentCommand command);
    }
}