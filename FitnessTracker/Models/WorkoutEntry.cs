using System;
using System.Collections.Generic;
using System.Text;

namespace FitnessTracker.Models
{
    public class WorkoutEntry
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public int Duration { get; set; }

        public string ActivityType { get; set; } = string.Empty;

        public int Sets { get; set; }

        public int Reps { get; set; }

        public double Weight { get; set; }

        public IntensityLevel IntensityLevel { get; set; }

        public int CaloriesBurned { get; set; }

        public bool IsRecurring { get; set; }

        public Frequency Frequency { get; set; }

    }

    public enum IntensityLevel
    {
        Low,
        Medium,
        High
    }

    public enum Frequency
    {
        Daily,
        Weekly,
        Monthly
    }

}
