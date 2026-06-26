using AstroMonitor.Application;
using AstroMonitor.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// DI
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
