using CommandService.Enums;

namespace CommandService.Models.Outbox
{
    public class EnrollmentOutbox
    {
        public string AggregateId { get; set; }
        public EventStatus Status { get; set; }
    }
}