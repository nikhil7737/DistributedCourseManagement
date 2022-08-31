using CommandService.Enums;

namespace CommandService.Models.Outbox
{
    public class CourseOutbox
    {
        public string CourseId { get; set; }
        public EventStatus Status { get; set; }
    }
}