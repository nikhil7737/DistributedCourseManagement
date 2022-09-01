using CommandService.Models.Commands;
using CommandService.Models.Events;

namespace CommandService.Repository.Interfaces;

public interface IEnrollmentRepo
{
    Task PesistEnrollmentEvent(EnrollmentEvent enrollmentEvent);
}
