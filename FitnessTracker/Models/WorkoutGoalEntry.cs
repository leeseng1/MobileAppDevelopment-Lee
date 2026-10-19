using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FitnessTracker.Models
{
    public class WorkoutGoalEntry : INotifyPropertyChanged
    {
        private GoalType type;
        private double startingValue;
        private double targetValue;
        private double currentValue;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }

        public string Title { get; set; } = string.Empty;
          
        public GoalType Type
        {
            get => type;
            set
            {
                if (type == value)
                    return;

                type = value;
                NotifyGoalStateChanged(nameof(Type));
            }
        }

        // TargetValue represents the goal value the user wants to achieve (e.g., target weight, target muscle mass, etc.)
        public string Unit { get; set; } = string.Empty;

        public double StartingValue
        {
            get => startingValue;
            set
            {
                if (startingValue == value)
                    return;

                startingValue = value;
                NotifyGoalStateChanged(nameof(StartingValue));
            }
        }

        public double TargetValue
        {
            get => targetValue;
            set
            {
                if (targetValue == value)
                    return;

                targetValue = value;
                NotifyGoalStateChanged(nameof(TargetValue));
            }
        }

        public double CurrentValue
        {
            get => currentValue;
            set
            {
                if (currentValue == value)
                    return;

                currentValue = value;
                NotifyGoalStateChanged(nameof(CurrentValue));
            }
        }

        public bool IsAchieved => Type == GoalType.WeightLoss? CurrentValue <= TargetValue : CurrentValue >= TargetValue;

        public double Progress
        {
            get
            {
                if (IsAchieved)
                    return 1;

                var totalChange = Type == GoalType.WeightLoss
                    ? StartingValue - TargetValue
                    : TargetValue - StartingValue;

                if (totalChange <= 0)
                    return 0;

                var completedChange = Type == GoalType.WeightLoss
                    ? StartingValue - CurrentValue
                    : CurrentValue - StartingValue;

                return Math.Clamp(completedChange / totalChange, 0, 1);
            }
        }

        public double ProgressPercent => Progress * 100;

        private void NotifyGoalStateChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAchieved)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Progress)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressPercent)));
        }

    }

    public enum GoalType
    {
        WeightLoss,
        MuscleGain,
        Endurance,
    }
}
