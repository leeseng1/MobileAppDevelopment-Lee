using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;

public partial class FoodLogPage : ContentPage
{
    private readonly FoodLogViewModel foodLogViewModel;

    public FoodLogPage(FoodLogViewModel foodLogViewModel)
	{
		InitializeComponent();

        this.foodLogViewModel = foodLogViewModel;

        BindingContext = foodLogViewModel;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        foodLogViewModel.LoadFoodEntriesCommand.Execute(null);
    }
}