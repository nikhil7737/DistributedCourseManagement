using CommandService.Enums;
using CommandService.Models.Entity;

namespace CommandService.Models.Events;

public class EnrollmentEvent
{
    public string AggregateId { get; set; }
    public int SequenceNo { get; set; }
    public EnrollmentType Type { get; set; }
    public Enrollment Enrollment { get; set; }
}