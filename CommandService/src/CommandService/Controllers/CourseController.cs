using Amazon.DynamoDBv2.DataModel;
using CommandService.BL.Interfaces;
using CommandService.ExtensionMethods;
using CommandService.Models.Commands;
using CommandService.Models.Entity;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;


[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly ICourseBL _courseBL;

    public CourseController(ICourseBL courseBL)
    {
        _courseBL = courseBL;
    }
    [HttpPost]
    public async Task Post(CreateCourseCommand createCommand)
    {
        if (ValidateCreateCourseCommand(createCommand))
        {
            await _courseBL.Execute(createCommand);
        }
    }

    [HttpDelete]
    public void Delete(CreateCourseCommand createCommand)
    {
    }

    private static bool ValidateCreateCourseCommand(CreateCourseCommand createCommand)
    {
        return !(createCommand.Name.IsNullOrEmpty() || createCommand.Description.IsNullOrEmpty() || createCommand.InstructorId.IsNullOrEmpty());
    }
}