using EventHighway.Core.Clients.EventHighways;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IEventHighwayClient>(serviceProvider =>
{
    string connectionString = builder.Configuration
        .GetConnectionString("EventHighwayDB");

    return new EventHighwayClient(connectionString);
});

var app = builder.Build();

app.MapControllers();

app.Run();
