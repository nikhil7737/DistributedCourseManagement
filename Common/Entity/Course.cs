namespace Common.Entity
{
    public class Course
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string InstructorId { get; set; }
        public int Fees { get; set; }
        public int MaxEnrollment { get; set; }
    }
}