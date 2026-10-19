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

        private readonly IWorkoutGoalStore workoutGoalStore;

        public DateTime CurrentDate { get; } = DateTime.Now;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<WorkoutEntry> WorkoutEntries { get; set; } = new();

        public ObservableCollection<WorkoutGoalCardViewModel> WorkoutGoals { get; } = new();

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

        // Commands
        public Command LoadWorkoutsCommand { get; }

        public Command<double> SaveWorkoutCommand { get; }

        public WorkoutLogViewModel(IDataService dataService, IWorkoutGoalStore workoutGoalStore)
        {
            this.dataService = dataService;
            this.workoutGoalStore = workoutGoalStore;

            LoadWorkoutsCommand = new Command(async () => await LoadWorkoutsAsync());

            // SaveWorkoutCommand is executed when the user swipes the thumb far enough to trigger a save action.
            SaveWorkoutCommand = new Command<double>(async progress =>
            {
                if (progress >= 0.88)
                {
                    await SaveWorkoutAsync();
                }
            });
        }

        // Load workouts and goals from the data service
        private async Task LoadWorkoutsAsync()
        {
            var workouts = await dataService.GetWorkoutEntriesAsync();
            var goals = await workoutGoalStore.GetWorkoutGoalsAsync();

            WorkoutEntries.Clear();
            foreach (var workout in workouts)
            {
                WorkoutEntries.Add(workout);
            }

            WorkoutGoals.Clear();
            foreach (var goal in goals)
            {
                WorkoutGoals.Add(new WorkoutGoalCardViewModel(goal, SaveGoalProgressAsync));
            }
        }

        // Save a new workout entry after validating the input fields and clear the input fields
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

        // Save the progress for a specific workout goal after validating the input value
        private async Task SaveGoalProgressAsync(WorkoutGoalCardViewModel card)
        {
            card.ErrorMessage = string.Empty;

            if (!double.TryParse(card.ProgressInput, out var newValue) ||
                !double.IsFinite(newValue) ||
                newValue < 0)
            {
                card.ErrorMessage = $"Enter a valid non-negative value in {card.Goal.Unit}.";
                return;
            }

            // Save a separate candidate first so the displayed goal stays unchanged
            // if SQLite reports a failure.
            var updatedGoal = new WorkoutGoalEntry
            {
                Id = card.Goal.Id,
                Title = card.Goal.Title,
                Type = card.Goal.Type,
                Unit = card.Goal.Unit,
                StartingValue = card.Goal.StartingValue,
                TargetValue = card.Goal.TargetValue,
                CurrentValue = newValue,
                StartDate = card.Goal.StartDate,
                EndDate = card.Goal.EndDate
            };

            try
            {
                await workoutGoalStore.UpdateWorkoutGoalAsync(updatedGoal);
            }
            catch (Exception)
            {
                card.ErrorMessage = "Could not save progress. Please try again.";
                return;
            }

            // Update the displayed goal only after the database save succeeds.
            card.Goal.CurrentValue = newValue;
            card.ProgressInput = string.Empty;
        }

        // Notify property changed for a given property name
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
