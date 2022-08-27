using CommandService.Models.Commands;
using CommandService.Models.Entity;
using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;

[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    [HttpPost]
    public void Post(CreateCourseCommand createCommand)
    {
        
    }

    [HttpDelete]
    public void Delete(DeleteCourseCommand deleteCommand)
    {
    }
}