namespace CommandService.Models.Events
{
    public class CourseCreatedEvent
    {
        public string EventId { get; set; }
        public string CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string InstructorId { get; set; }
    }
}