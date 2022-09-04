using Amazon.DynamoDBv2.DataModel;
using CommandService.BL.Interfaces;
using Common.ExtensionMethods;
using CommandService.Models.Commands;
using Common.Entity;
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
    public async Task Delete(DeleteCourseCommand deleteCommand)
    {
        if (ValidateDeleteCourseCommand(deleteCommand))
        {
            await _courseBL.Execute(deleteCommand);
        }
    }

    private static bool ValidateCreateCourseCommand(CreateCourseCommand createCommand)
    {
        return !(createCommand.Name.IsNullOrEmpty() || createCommand.Description.IsNullOrEmpty() || createCommand.InstructorId.IsNullOrEmpty());
    }
    private static bool ValidateDeleteCourseCommand(DeleteCourseCommand deleteCommand)
    {
        return !deleteCommand.CourseId.IsNullOrEmpty();
    }
}