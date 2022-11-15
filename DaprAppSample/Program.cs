using Dapr;
using Dapr.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();

var app = builder.Build();

app.UseCloudEvents();
app.MapSubscribeHandler();

app.MapGet("/hello", () => {
        var daprClient = app.Services.GetRequiredService<DaprClient>();
        daprClient.PublishEventAsync("pubsub", "message", new HelloRecord(Guid.NewGuid(),"Hello Dapr!"));
        return Results.Ok("Hello Dapr!");
    });

app.MapPost("/checkout", [Topic("pubsub","message")] (HelloRecord record) => {
    Console.WriteLine($"Subscriber received a record with ID: {record.Id} and Text: {record.Message}");
    return Results.Ok();
});

app.Run();

record HelloRecord(Guid Id, string Message);
