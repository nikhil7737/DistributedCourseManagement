using Amazon.DynamoDBv2.DataModel;
using Common.ReadModels;
using QueryService.Repo.Interface;

namespace QueryService.Repo;

public class CourseRepo : ICourseRepo
{
    private readonly IDynamoDBContext _dbContext;
    public CourseRepo(IDynamoDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<T>> GetCourseByHashKey<T>(string hashKey)
    {
        AsyncSearch<T> response = _dbContext.QueryAsync<T>(hashKey);
        return await response.GetRemainingAsync();
    }
}