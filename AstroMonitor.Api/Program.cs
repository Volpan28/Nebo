using AstroMonitor.Api.ExceptionHandlers;
using AstroMonitor.Application;
using AstroMonitor.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// DI
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
