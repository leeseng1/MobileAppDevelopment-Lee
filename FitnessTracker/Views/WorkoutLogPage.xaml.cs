using FitnessTracker.ViewModels;
using System.Security.Cryptography.X509Certificates;

namespace FitnessTracker.Views;

public partial class WorkoutLogPage : ContentPage
{
	private readonly WorkoutLogViewModel viewModel;

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

    // Handle the pan gesture for the swipe-to-save functionality, 
    // updating the position of the thumb and the progress fill based on the user's swipe.
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

            // When the gesture is completed or canceled, check if the swipe was sufficient to trigger the save action.
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                {
                    var maxX = Math.Max(0, SwipeTrack.Width - SwipeThumb.Width - SwipeThumb.Margin.Left - SwipeThumb.Margin.Right);

                    // If the swipe was completed and the thumb was dragged far enough, execute the save command with the calculated progress.
                    if (e.StatusType == GestureStatus.Completed && maxX > 0 && SwipeThumb.TranslationX / maxX >= 0.88)
                    {
                        var progress = Math.Clamp(SwipeThumb.TranslationX / maxX, 0, 1);
                        viewModel.SaveWorkoutCommand.Execute(progress);
                    }

                    await SwipeThumb.TranslateToAsync(0, 0, 220, Easing.CubicOut);
                    ProgressFill.WidthRequest = 0;
                    ProgressFill.Opacity = 0;

                    break;
                }
        }
    }
}