using Amazon.DynamoDBv2.DataModel;
using CommandService.BL.Interfaces;
using Common.ExtensionMethods;
using CommandService.Models.Commands;
using Common.Entity;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;


[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentBL _enrollmentBL;

    public EnrollmentController(IEnrollmentBL enrollmentBL)
    {
        _enrollmentBL = enrollmentBL;
    }
    [HttpPost]
    public async Task<IActionResult> UpdateEnrollment(EnrollmentCommand command)
    {
        if (command.CourseId.IsNullOrEmpty() || command.StudentId.IsNullOrEmpty())
        {
            return BadRequest();
        }
        await _enrollmentBL.Execute(command);
        return Ok();
    }
}