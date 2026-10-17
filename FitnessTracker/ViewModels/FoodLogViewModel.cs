using FitnessTracker.Services.Interfaces;
using System.ComponentModel;
using FitnessTracker.Models;
using System.Collections.ObjectModel;

namespace FitnessTracker.ViewModels
{
    public class FoodLogViewModel : INotifyPropertyChanged
    {
        private readonly IDataService dataService;

        public DateTime CurrentDate { get; } = DateTime.Now;

        private string errorMessge = string.Empty;

        private string dailyTotals = string.Empty;

        private DietGoalEntry dietGoal = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<FoodEntry> FoodEntries { get; set; } = new();

        public IReadOnlyList<MealType> Mealtypes { get; } = Enum.GetValues<MealType>();

        //properties for the input fields
        public string NameInput { get; set; } = string.Empty;

        public string CaloriesInput { get; set; } = string.Empty;

        public string ProteinInput { get; set; } = string.Empty;

        public string CarbsInput { get; set; } = string.Empty;

        public string FatsInput { get; set; } = string.Empty;

        public MealType SelectedMeal { get; set; } = MealType.Breakfast;

        public string ErrorMessage
        {
            get => errorMessge;
            private set
            {
                if (errorMessge == value)
                {
                    return;
                }
                errorMessge = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public string DailyTotals
        {
            get => dailyTotals;
            private set
            {
                if (dailyTotals == value)
                {
                    return;
                }
                dailyTotals = value;
                OnPropertyChanged(nameof(DailyTotals));
            }
        }

        public string CaloriesGoalText =>
            $"Calories: {FoodEntries.Sum(entry => entry.Calories)} / {dietGoal.TargetCalories:0} cal";

        public string ProteinGoalText =>
            $"Protein: {FoodEntries.Sum(entry => entry.Protein):0.#} / {dietGoal.TargetProtein:0.#} g";

        public string CarbsGoalText =>
            $"Carbs: {FoodEntries.Sum(entry => entry.Carbs):0.#} / {dietGoal.TargetCarbs:0.#} g";

        public string FatsGoalText =>
            $"Fats: {FoodEntries.Sum(entry => entry.Fats):0.#} / {dietGoal.TargetFats:0.#} g";

        public double CaloriesProgress =>
            GetProgress(FoodEntries.Sum(entry => entry.Calories), dietGoal.TargetCalories);

        public double ProteinProgress =>
            GetProgress(FoodEntries.Sum(entry => entry.Protein), dietGoal.TargetProtein);

        public double CarbsProgress =>
            GetProgress(FoodEntries.Sum(entry => entry.Carbs), dietGoal.TargetCarbs);

        public double FatsProgress =>
            GetProgress(FoodEntries.Sum(entry => entry.Fats), dietGoal.TargetFats);

        public Command LoadFoodEntriesCommand { get; }

        public Command SaveFoodEntryCommand { get; }

        public FoodLogViewModel(IDataService dataService)
        {
            this.dataService = dataService;
            LoadFoodEntriesCommand = new Command(async () => await LoadFoodEntriesAsync());
            SaveFoodEntryCommand = new Command(async () => await SaveFoodEntryAsync());
            UpdateDailyTotals();
        }

        private async Task LoadFoodEntriesAsync()
        {
            dietGoal = await dataService.GetDietGoalAsync();

            var entries = await dataService.GetFoodEntriesAsync();

            FoodEntries.Clear();

            foreach (var entry in entries
                 .Where(entry => entry.Date.Date == CurrentDate.Date)
                 .OrderByDescending(entry => entry.Date))
            {
                FoodEntries.Add(entry);
            }

            UpdateDailyTotals();
            NotifyGoalProperties();
        }

        private async Task SaveFoodEntryAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(NameInput))
            {
                ErrorMessage = "Enter a food or drink name.";
                return;
            }

            if (!int.TryParse(CaloriesInput, out var calories) || calories < 0)
            {
                ErrorMessage = "Calories must be zero or greater.";
                return;
            }

            if (!double.TryParse(ProteinInput, out var protein) || protein < 0 ||
                !double.TryParse(CarbsInput, out var carbs) || carbs < 0 ||
                !double.TryParse(FatsInput, out var fats) || fats < 0)
            {
                ErrorMessage = "Protein, carbs, and fats must be zero or greater.";
                return;
            }

            var foodEntry = new FoodEntry
            {
                Date = CurrentDate,
                Meal = SelectedMeal,
                Name = NameInput.Trim(),
                Calories = calories,
                Protein = protein,
                Carbs = carbs,
                Fats = fats
            };

            await dataService.SaveFoodEntryAsync(foodEntry);
            FoodEntries.Insert(0, foodEntry);
            UpdateDailyTotals();
            NotifyGoalProperties();

            NameInput = string.Empty;
            CaloriesInput = string.Empty;
            ProteinInput = string.Empty;
            CarbsInput = string.Empty;
            FatsInput = string.Empty;

            OnPropertyChanged(nameof(NameInput));
            OnPropertyChanged(nameof(CaloriesInput));
            OnPropertyChanged(nameof(ProteinInput));
            OnPropertyChanged(nameof(CarbsInput));
            OnPropertyChanged(nameof(FatsInput));
        }

        private void UpdateDailyTotals()
        {
            DailyTotals =
                $"Daily Progress — Calories: {FoodEntries.Sum(entry => entry.Calories)}, " +
                $"Protein: {FoodEntries.Sum(entry => entry.Protein):0.#} g, " +
                $"Carbs: {FoodEntries.Sum(entry => entry.Carbs):0.#} g, " +
                $"Fats: {FoodEntries.Sum(entry => entry.Fats):0.#} g";
        }

        private void NotifyGoalProperties()
        {
            OnPropertyChanged(nameof(CaloriesGoalText));
            OnPropertyChanged(nameof(ProteinGoalText));
            OnPropertyChanged(nameof(CarbsGoalText));
            OnPropertyChanged(nameof(FatsGoalText));
            OnPropertyChanged(nameof(CaloriesProgress));
            OnPropertyChanged(nameof(ProteinProgress));
            OnPropertyChanged(nameof(CarbsProgress));
            OnPropertyChanged(nameof(FatsProgress));
        }

        private static double GetProgress(double current, double target) =>
            target <= 0 ? 0 : Math.Clamp((double)current / target, 0, 1);

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
}
