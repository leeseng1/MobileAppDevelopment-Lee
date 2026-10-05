using FitnessTracker.ViewModels;
using System.Security.Cryptography.X509Certificates;

namespace FitnessTracker.Views;

public partial class WorkoutLogPage : ContentPage
{
	private readonly WorkoutLogViewModel viewModel;

    private double panStartX;

    public WorkoutLogPage(WorkoutLogViewModel viewModel)
	{
		InitializeComponent();

        this.viewModel = viewModel;

        BindingContext = viewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        
        viewModel.LoadWorkoutsCommand.Execute(null);
    }

    private async void OnSwipePanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                panStartX = SwipeThumb.TranslationX;
                break;

            case GestureStatus.Running:
            {
                var maxX = Math.Max(0, SwipeTrack.Width - SwipeThumb.Width - SwipeThumb.Margin.Left - SwipeThumb.Margin.Right);

                var x = Math.Clamp(panStartX + e.TotalX, 0, maxX);

                var progress = maxX == 0 ? 0 : x / maxX;

                SwipeThumb.TranslationX = x;

                ProgressFill.WidthRequest = Math.Min(SwipeTrack.Width - 8, x + SwipeThumb.Width);

                ProgressFill.Opacity = progress;

                break;
            }

            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                {
                    var maxX = Math.Max(0, SwipeTrack.Width - SwipeThumb.Width - SwipeThumb.Margin.Left - SwipeThumb.Margin.Right);

                    if (e.StatusType == GestureStatus.Completed
                        && maxX > 0
                        && SwipeThumb.TranslationX / maxX >= 0.88)
                    {
                        viewModel.SaveWorkoutCommand.Execute(null);
                    }

                    await SwipeThumb.TranslateToAsync(0, 0, 220, Easing.CubicOut);
                    ProgressFill.WidthRequest = 0;
                    ProgressFill.Opacity = 0;

                    break;
                }
        }
    }
}