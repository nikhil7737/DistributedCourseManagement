using CommandService.BL.Interfaces;
using Common.Enums;
using CommandService.Models.Commands;
using Common.Events;
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
                CourseId = command.CourseId,
                Date = DateTime.Now,
            }
        };
        await _repo.PesistEnrollmentEvent(enrollmentEvent);
    }
}