using FitnessTracker.ViewModels;
using System.Security.Cryptography.X509Certificates;

namespace FitnessTracker.Views;

public partial class WorkoutLogPage : ContentPage
{
	public WorkoutLogPage(WorkoutLogViewModel viewModel)
	{
		InitializeComponent();

		BindingContext = viewModel;
	}
}