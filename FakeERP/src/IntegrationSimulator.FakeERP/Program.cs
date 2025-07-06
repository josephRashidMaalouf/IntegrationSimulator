using IntegrationSimulator.FakeERP;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/", (List<JobPostedRequestDto> dto) =>
{
    foreach (var job in dto)
    {
        Console.WriteLine($"Pretending to do stuff with this data for ad: {job.Id}");
    }
    return Results.Ok();
});

app.Run();
