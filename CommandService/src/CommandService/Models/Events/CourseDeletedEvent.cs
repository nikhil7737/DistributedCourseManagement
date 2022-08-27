namespace CommandService.Models.Events
{
    public class CourseDeletedEvent
    {
        public string EventId { get; set; }
        public string CourseId { get; set; }
    }
}