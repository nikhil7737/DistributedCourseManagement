using CommandService.BL.Interfaces;
using CommandService.Enums;
using CommandService.Models.Commands;
using CommandService.Models.Events;
using CommandService.Repository.Interfaces;

namespace CommandService.BL;

public class EnrollmentBL : IEnrollmentBL
{
    private readonly ICourseRepo _repo;
    public EnrollmentBL(ICourseRepo repo)
    {
        _repo = repo;
    }
    public async Task Execute(EnrollmentCommand command)
    {
        
    }
}