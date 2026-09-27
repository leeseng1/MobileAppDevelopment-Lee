using FitnessTracker.ViewModels;

namespace FitnessTracker.Views;

public partial class FoodLogPage : ContentPage
{
	public FoodLogPage(FoodLogViewModel foodLogViewModel)
	{
		InitializeComponent();

        BindingContext = foodLogViewModel;
	}
}