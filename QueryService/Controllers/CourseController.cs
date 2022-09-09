using Common.ReadModels;
using Microsoft.AspNetCore.Mvc;
using QueryService.Repo.Interface;

namespace CommandService.Controllers;


[ApiController]
[Route("query/courses/by")]
public class CourseController : ControllerBase
{
    private readonly ICourseRepo _repo;

    public CourseController(ICourseRepo repo)
    {
        _repo = repo;
    }

    [HttpGet("instructor")]
    public async Task<IActionResult> GetCoursesByInstructorId(string instructorId)
    {
        try
        {
            List<CourseByInstructor> courses = await _repo.GetCourseByHashKey<CourseByInstructor>(instructorId);
            return Ok(courses);
        }
        catch (Exception e)
        {
            return Ok(e);
        }
    }

    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetCoursesByStudentId(string studentId)
    {
        try
        {
            List<CourseByStudent> courses = await _repo.GetCourseByHashKey<CourseByStudent>(studentId);
            return Ok(courses);
        }
        catch (Exception e)
        {
            return Ok(e);
        }
    }
}