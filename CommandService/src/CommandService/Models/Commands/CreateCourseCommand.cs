namespace CommandService.Models.Commands
{
    public class CreateCourseCommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string InstructorId { get; set; }
    }
}