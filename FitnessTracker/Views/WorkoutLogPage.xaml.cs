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
}