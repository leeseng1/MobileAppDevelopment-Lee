using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;

public partial class GoalsPage : ContentPage
{
	public GoalsPage(GoalsViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;

    }

}