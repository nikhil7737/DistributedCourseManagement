using Amazon.Lambda.Core;
using Common.EBModels;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CoursesByStudentProjector;

public class CoursesByStudentProjector
{
    public async Task Project(EBEvent ebEvent, ILambdaContext context)
    {
        
    }
}
