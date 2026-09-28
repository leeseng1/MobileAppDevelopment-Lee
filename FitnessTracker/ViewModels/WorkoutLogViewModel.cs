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

        public DateTime CurrentDate { get; } = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<WorkoutEntry> WorkoutEntries { get; set; } = new();

        public ObservableCollection<WorkoutGoalEntry> WorkoutGoals { get; } = new();

        public IReadOnlyList<IntensityLevel> IntensityLevels { get;  } = Enum.GetValues<IntensityLevel>();  

        public IReadOnlyList<Frequency> Frequencies { get; } = Enum.GetValues<Frequency>();

        public string ActivityTypeInput { get; set; } = string.Empty;

        public string DurationInput { get; set; } = string.Empty;

        public string CaloriesInput { get; set; } = string.Empty;

        public IntensityLevel SelectedIntensity { get; set; } = IntensityLevel.Medium;

        private bool isRecurring;
        public bool IsRecurring { 
            get => isRecurring;
            set
            {
                if (isRecurring == value)
                {
                    return;
                }
                isRecurring = value;
                OnPropertyChanged(nameof(IsRecurring));
            }
        }

        public Frequency SelectedFrequency { get; set; } = Frequency.Weekly;

        private bool hasReminder;
        public bool HasReminder
        {
            get => hasReminder;
            set
            {
                if (hasReminder == value)
                {
                    return;
                }
                hasReminder = value;
                OnPropertyChanged(nameof(HasReminder));
            }
        }

        public DateTime ReminderDate { get; set; } = DateTime.Today.AddDays(1);

        public TimeSpan ReminderTime { get; set; } = new(9, 0, 0);

        private string errorMessage = string.Empty;
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
            var goals = await dataService.GetWorkoutGoalsAsync();

            WorkoutEntries.Clear();
            foreach (var workout in workouts)
            {
                WorkoutEntries.Add(workout);
            }

            WorkoutGoals.Clear();
            foreach (var goal in goals)
            {
                WorkoutGoals.Add(goal);
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

            var reminderDateTime = ReminderDate.Date + ReminderTime;

            if(HasReminder && reminderDateTime <= DateTime.Now)
            {
                ErrorMessage = "Reminder date and time must be in the future";
                return;
            }

            var workout = new WorkoutEntry
            {
                ActivityType = ActivityTypeInput.Trim(),
                CurrentDate = CurrentDate,
                Duration = duration,
                IntensityLevel = SelectedIntensity,
                CaloriesBurned = calories,
                IsRecurring = IsRecurring,
                Frequency = SelectedFrequency,
                HasReminder = HasReminder,
                ReminderDateTime = reminderDateTime
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
