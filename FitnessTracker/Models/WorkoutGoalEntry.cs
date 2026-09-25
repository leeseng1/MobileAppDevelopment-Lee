using System;
using System.Collections.Generic;
using System.Text;

namespace FitnessTracker.Models
{
    public class WorkoutGoalEntry
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }

        public string Title { get; set; } = string.Empty;
        public GoalType Type { get; set; }

        // TargetValue represents the goal value the user wants to achieve (e.g., target weight, target muscle mass, etc.)
        public double TargetValue { get; set; }
        public double CurrentValue { get; set; }

        public bool IsAchieved => CurrentValue >= TargetValue;

    }

    public enum GoalType
    {
        WeightLoss,
        MuscleGain,
        Endurance,
    }
}
