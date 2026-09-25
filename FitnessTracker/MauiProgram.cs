using FitnessTracker.Services;
using FitnessTracker.Services.Interfaces;
using FitnessTracker.ViewModels;
using FitnessTracker.Views;
using Microsoft.Extensions.Logging;

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

            builder.Services.AddTransient<WorkoutLogViewModel>();

            builder.Services.AddTransient<WorkoutLogPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
