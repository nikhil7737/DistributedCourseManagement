using CommandService.Enums;

namespace CommandService.Models.Commands;

public class EnrollmentCommand
{
    public string StudentId { get; set; }
    public string CourseId { get; set; }
    public EnrollmentType Type { get; set; }
}