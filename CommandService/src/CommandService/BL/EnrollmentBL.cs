using CommandService.BL.Interfaces;
using CommandService.Enums;
using CommandService.Models.Commands;
using CommandService.Models.Events;
using CommandService.Repository.Interfaces;

namespace CommandService.BL;

public class EnrollmentBL : IEnrollmentBL
{
    private readonly IEnrollmentRepo _repo;
    public EnrollmentBL(IEnrollmentRepo repo)
    {
        _repo = repo;
    }
    public async Task Execute(EnrollmentCommand command)
    {
        string aggregateId = $"{command.StudentId}-{command.CourseId}";
        var enrollmentEvent = new EnrollmentEvent
        {
            AggregateId = aggregateId,
            SequenceNo = await _repo.GetLastSequenceNo(aggregateId) + 1,
            Type = command.Type,
            Enrollment = new () {
                StudentId = command.StudentId,
                CourseId = command.CourseId
            }
        };
        await _repo.PesistEnrollmentEvent(enrollmentEvent);
    }
}