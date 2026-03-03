using EventListenerApi.Brokers.Storages;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

string connectionString = builder.Configuration.GetConnectionString("SchoolDB");
builder.Services.AddTransient<IStorageBroker, StorageBroker>(provider => 
    new StorageBroker(connectionString));

var app = builder.Build();

app.MapControllers();

app.Run();
