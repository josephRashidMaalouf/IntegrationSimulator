using IntegrationSimulator.FakeERP;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/", async (List<JobPostedRequestDto> dto) =>
{
    //Simulate ERP down
    var random = new Random();
    var rNum = random.Next(11);
    if (rNum > 5)
    {
        return Results.StatusCode(503);
    }

    Console.WriteLine($"Received: {dto.Count} number of listings");

    var path = Path.Combine(Environment.CurrentDirectory, "jobads.csv");

    await File.AppendAllLinesAsync(path, dto.Select(x => x.ToString()));


    return Results.Ok();
});

app.Run();