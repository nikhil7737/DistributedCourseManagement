using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using CommandService.BL;
using CommandService.BL.Interfaces;
using CommandService.Repository;
using CommandService.Repository.Interfaces;

namespace CommandService;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddAWSService<IAmazonDynamoDB>();
        services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
        var chain = new CredentialProfileStoreChain();
        AWSCredentials awsCredentials;
        chain.TryGetAWSCredentials("Nikhil", out awsCredentials);
        services.AddSingleton<IAmazonDynamoDB>(new AmazonDynamoDBClient(awsCredentials));

        services.AddScoped<ICourseBL, CourseBL>();
        services.AddScoped<ICourseRepo, CourseRepo>();
        services.AddScoped<IEnrollmentBL, EnrollmentBL>();
        services.AddScoped<IEnrollmentRepo, EnrollmentRepo>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}