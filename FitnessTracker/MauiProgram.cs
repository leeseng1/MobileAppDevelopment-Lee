using FitnessTracker.Services;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.ViewModels;
using FitnessTracker.Views;
using Microsoft.Extensions.Logging;
using FitnessTracker.Data;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<IDataService, MockDataService>();

            // Register the FitnessTrackerDbContext with a SQLite database in the app's data directory
            builder.Services.AddDbContextFactory<FitnessTrackerDbContext>(options => options.UseSqlite(
                $"Data Source={Path.Combine(FileSystem.AppDataDirectory, "fitness-tracker.db3")}"));

            builder.Services.AddSingleton<IWorkoutGoalStore, WorkoutGoalStore>();

            builder.Services.AddTransient<WorkoutLogViewModel>();

            builder.Services.AddTransient<WorkoutLogPage>();

            builder.Services.AddTransient<FoodLogViewModel>();

            builder.Services.AddTransient<FoodLogPage>();

            builder.Services.AddTransient<GoalsViewModel>();

            builder.Services.AddTransient<GoalsPage>();



#if DEBUG
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Create the local schema before pages request workout goals.
            using (var scope = app.Services.CreateScope())
            {
                var factory = scope.ServiceProvider
                    .GetRequiredService<IDbContextFactory<FitnessTrackerDbContext>>();

                using var context = factory.CreateDbContext();
                context.Database.EnsureCreated();
            }

            return app;
        }
    }
}
