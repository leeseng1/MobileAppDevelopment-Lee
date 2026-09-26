using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace FitnessTracker.ViewModels
{
    public class WorkoutLogViewModel : INotifyPropertyChanged
    {
        private readonly IDataService dataService;

        private string errorMessage = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<WorkoutEntry> WorkoutEntries { get; set; } = new();

        public IReadOnlyList<IntensityLevel> IntensityLevels { get;  } = Enum.GetValues<IntensityLevel>();  

        public IReadOnlyList<Frequency> Frequencies { get; } = Enum.GetValues<Frequency>();

        public string ActivityTypeInput { get; set; } = string.Empty;
        public string DurationInput { get; set; } = string.Empty;
        public string CaloriesInput { get; set; } = string.Empty;
        public IntensityLevel SelectedIntensity { get; set; } = IntensityLevel.Medium;
        public bool IsRecurring { get; set; }
        public Frequency SelectedFrequency { get; set; } = Frequency.Weekly;

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

        public Command LoadWorkoutsCommand { get; }

        public Command SaveWorkoutCommand { get; }

        public WorkoutLogViewModel(IDataService dataService)
        {
            this.dataService = dataService;

            LoadWorkoutsCommand = new Command(async () => await LoadWorkoutsAsync());
            SaveWorkoutCommand = new Command(async () => await SaveWorkoutAsync());
        }

        private async Task LoadWorkoutsAsync()
        {
            var workouts = await dataService.GetWorkoutEntriesAsync();
            WorkoutEntries.Clear();
            foreach (var workout in workouts)
            {
                WorkoutEntries.Add(workout);
            }
        }

        private async Task SaveWorkoutAsync()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(ActivityTypeInput))
            {
                ErrorMessage = "Enter an activity type.";
                return;
            }

            if (!int.TryParse(DurationInput, out var duration) || duration <= 0)
            {
                ErrorMessage = "Duration must be a number greater than zero.";
                return;
            }

            if (!int.TryParse(CaloriesInput, out var calories) || calories < 0)
            {
                ErrorMessage = "Calories must be zero or greater.";
                return;
            }

            var workout = new WorkoutEntry
            {
                ActivityType = ActivityTypeInput.Trim(),
                Duration = duration,
                IntensityLevel = SelectedIntensity,
                CaloriesBurned = calories,
                IsRecurring = IsRecurring,
                Frequency = SelectedFrequency
            };

            await dataService.SaveWorkoutAsync(workout);
            WorkoutEntries.Insert(0, workout);

            ActivityTypeInput = string.Empty;
            DurationInput = string.Empty;
            CaloriesInput = string.Empty;
            IsRecurring = false;

            OnPropertyChanged(nameof(ActivityTypeInput));
            OnPropertyChanged(nameof(DurationInput));
            OnPropertyChanged(nameof(CaloriesInput));
            OnPropertyChanged(nameof(IsRecurring));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
