using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;

public partial class GoalsPage : ContentPage
{
    private readonly GoalsViewModel viewModel;

    public GoalsPage(GoalsViewModel viewModel)
    {
        InitializeComponent();

        this.viewModel = viewModel;

        BindingContext = viewModel;

    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        viewModel.LoadGoalsCommand.Execute(null);
    }

}