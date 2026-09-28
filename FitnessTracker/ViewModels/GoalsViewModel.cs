using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace FitnessTracker.ViewModels
{
    public class GoalsViewModel : INotifyPropertyChanged
    {
        private readonly IDataService dataService;

        private string errorMessage = string.Empty;

        private string statusMessage = string.Empty;

        private DietGoalEntry dietGoal = new();

        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<WorkoutGoalEntry> WorkoutGoals { get; } = new();
        public IReadOnlyList<GoalType> GoalTypes { get; } =
            Enum.GetValues<GoalType>();

        // Properties for the diet goal input fields
        public string CaloriesTargetInput { get; set; } = string.Empty;
        public string ProteinTargetInput { get; set; } = string.Empty;
        public string CarbsTargetInput { get; set; } = string.Empty;
        public string FatsTargetInput { get; set; } = string.Empty;
        public string MealsTargetInput { get; set; } = string.Empty;

        // Property to display the current diet goal summary
        public string DietGoalSummary =>
            $"Calories: {dietGoal.TargetCalories:0} cal\n" +
            $"Protein: {dietGoal.TargetProtein:0.#} g\n" +
            $"Carbs: {dietGoal.TargetCarbs:0.#} g\n" +
            $"Fats: {dietGoal.TargetFats:0.#} g\n" +
            $"Meals per day: {dietGoal.TargetMealsPerDay}";

        // Properties for the workout goal input fields
        public string WorkoutGoalTitleInput { get; set; } = string.Empty;
        public string WorkoutTargetInput { get; set; } = string.Empty;
        public string WorkoutCurrentInput { get; set; } = string.Empty;
        public GoalType SelectedGoalType { get; set; } = GoalType.Endurance;
        public DateTime WorkoutStartDate { get; set; } = DateTime.Today;
        public DateTime WorkoutEndDate { get; set; } = DateTime.Today.AddMonths(1);

        // Properties for error and status messages
        public string ErrorMessage
        {
            get => errorMessage;
            private set
            {
                if (errorMessage == value)
                {
                    return;
                }
                errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public string StatusMessage
        {
            get => statusMessage;
            private set
            {
                if (statusMessage == value)
                {
                    return;
                }
                statusMessage = value;
                OnPropertyChanged(nameof(StatusMessage));
            }
        }

        // Commands for loading and saving goals
        public Command LoadGoalsCommand { get; }
        public Command SaveDietGoalCommand { get; }
        public Command SaveWorkoutGoalCommand { get; }

        public GoalsViewModel(IDataService dataService)
        {
            this.dataService = dataService;

            LoadGoalsCommand = new Command(async () => await LoadGoalsAsync());
            SaveDietGoalCommand = new Command(async () => await SaveDietGoalAsync());
            SaveWorkoutGoalCommand = new Command(async () => await SaveWorkoutGoalAsync());
        }

        // Method to load diet and workout goals from the data service
        private async Task LoadGoalsAsync()
        {
            dietGoal = await dataService.GetDietGoalAsync();
            OnPropertyChanged(nameof(DietGoalSummary));

            OnPropertyChanged(nameof(CaloriesTargetInput));
            OnPropertyChanged(nameof(ProteinTargetInput));
            OnPropertyChanged(nameof(CarbsTargetInput));
            OnPropertyChanged(nameof(FatsTargetInput));
            OnPropertyChanged(nameof(MealsTargetInput));

            var goals = await dataService.GetWorkoutGoalsAsync();
            WorkoutGoals.Clear();

            foreach (var goal in goals)
            {
                WorkoutGoals.Add(goal);
            }
        }

        private async Task SaveDietGoalAsync()
        {
            ErrorMessage = string.Empty;
            StatusMessage = string.Empty;

            // Validate the input fields for diet goals
            if (!double.TryParse(CaloriesTargetInput, out var calories) || calories <= 0 ||
                !double.TryParse(ProteinTargetInput, out var protein) || protein < 0 ||
                !double.TryParse(CarbsTargetInput, out var carbs) || carbs < 0 ||
                !double.TryParse(FatsTargetInput, out var fats) || fats < 0 ||
                !int.TryParse(MealsTargetInput, out var meals) || meals <= 0)
            {
                ErrorMessage = "Enter positive values or 0 for none";
                return;
            }

            dietGoal = new DietGoalEntry
            {
                Id = dietGoal.Id,
                StartDate = dietGoal.StartDate,
                TargetCalories = calories,
                TargetProtein = protein,
                TargetCarbs = carbs,
                TargetFats = fats,
                TargetMealsPerDay = meals
            };

            await dataService.SaveDietGoalAsync(dietGoal);

            // Clear the input fields after saving
            CaloriesTargetInput = string.Empty;
            ProteinTargetInput = string.Empty;
            CarbsTargetInput = string.Empty;
            FatsTargetInput = string.Empty;
            MealsTargetInput = string.Empty;

            // Notify the UI to update the input fields and diet goal summary
            OnPropertyChanged(nameof(CaloriesTargetInput));
            OnPropertyChanged(nameof(ProteinTargetInput));
            OnPropertyChanged(nameof(CarbsTargetInput));
            OnPropertyChanged(nameof(FatsTargetInput));
            OnPropertyChanged(nameof(MealsTargetInput));
            OnPropertyChanged(nameof(DietGoalSummary));

            StatusMessage = "Diet goals saved";
        }

        private async Task SaveWorkoutGoalAsync()
        {
            ErrorMessage = string.Empty;
            StatusMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(WorkoutGoalTitleInput))
            {
                ErrorMessage = "Enter a title for the workout goal.";
                return;
            }

            if (!double.TryParse(WorkoutTargetInput, out var target) || target <= 0 ||
                !double.TryParse(WorkoutCurrentInput, out var current) || current < 0)
            {
                ErrorMessage = "Enter a positive target and a non-negative current value.";
                return;
            }

            if (WorkoutEndDate.Date < WorkoutStartDate.Date)
            {
                ErrorMessage = "The end date must be on or after the start date.";
                return;
            }

            var goal = new WorkoutGoalEntry
            {
                Title = WorkoutGoalTitleInput,
                Type = SelectedGoalType,
                TargetValue = target,
                CurrentValue = current,
                StartDate = WorkoutStartDate,
                EndDate = WorkoutEndDate
            };

            await dataService.SaveWorkoutGoalAsync(goal);
            WorkoutGoals.Add(goal);

            WorkoutGoalTitleInput = string.Empty;
            WorkoutTargetInput = string.Empty;
            WorkoutCurrentInput = string.Empty;

            OnPropertyChanged(nameof(WorkoutGoalTitleInput));
            OnPropertyChanged(nameof(WorkoutTargetInput));
            OnPropertyChanged(nameof(WorkoutCurrentInput));

            StatusMessage = "Workout goal saved.";
        }


        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
