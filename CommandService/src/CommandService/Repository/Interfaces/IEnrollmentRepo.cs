using CommandService.Models.Commands;
using Common.Events;

namespace CommandService.Repository.Interfaces;

public interface IEnrollmentRepo
{
    Task PesistEnrollmentEvent(EnrollmentEvent enrollmentEvent);
    Task<int> GetLastSequenceNo(string aggregateId);
}
