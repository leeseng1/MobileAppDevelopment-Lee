using System.Text.Json;
using System.Text.Json.Serialization;
using FitnessTracker.Api.Repositories;
using FitnessTracker.Api.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(options =>
    {
        // Validation error keys use JSON names (activityType, not ActivityType).
        options.ModelMetadataDetailsProviders.Add(
            new SystemTextJsonValidationMetadataProvider(JsonNamingPolicy.CamelCase));
    })
    .AddJsonOptions(options =>
    {
        // Enums travel as "Low"/"High" instead of 0/2.
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

// Repository is a singleton (it owns the file lock); service is scoped per request.
builder.Services.AddSingleton<IWorkoutRepository, JsonWorkoutRepository>();
builder.Services.AddScoped<IWorkoutService, WorkoutService>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();