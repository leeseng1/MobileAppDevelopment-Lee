using FitnessTracker.Models;
using System.ComponentModel;


namespace FitnessTracker.ViewModels
{
    /// <summary>
    /// holds the display temporary editing state for one workout goal card in the workout log page
    /// </summary>
    public sealed class WorkoutGoalCardViewModel : INotifyPropertyChanged
    {
        private readonly Func<WorkoutGoalCardViewModel, Task> saveProgress;
        private bool isExpanded;
        private string progressInput = string.Empty;
        private string errorMessage = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public WorkoutGoalEntry Goal { get; }

        public Command ToggleExpandedCommand { get; }
        public Command UpdateProgressCommand { get; }

        public bool IsExpanded
        {
            get => isExpanded;
            set => SetProperty(ref isExpanded, value);
        }

        public string ProgressInput
        {
            get => progressInput;
            set
            {
                if (!SetProperty(ref progressInput, value))
                    return;

                UpdateProgressCommand.ChangeCanExecute();
            }
        }

        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        public WorkoutGoalCardViewModel(WorkoutGoalEntry goal, Func<WorkoutGoalCardViewModel, Task> saveProgress)
        {
            Goal = goal ?? throw new ArgumentNullException(nameof(goal));
            this.saveProgress = saveProgress
                ?? throw new ArgumentNullException(nameof(saveProgress));

            ToggleExpandedCommand = new Command(
                () => IsExpanded = !IsExpanded);

            UpdateProgressCommand = new Command(
                async () => await this.saveProgress(this),
                () => !string.IsNullOrWhiteSpace(ProgressInput));
        }

        private bool SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}