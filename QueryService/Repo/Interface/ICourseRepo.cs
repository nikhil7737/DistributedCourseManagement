namespace QueryService.Repo.Interface;

public interface ICourseRepo
{
    Task<List<T>> GetCourseByHashKey<T>(string hashKey);
}