namespace FitnessTracker.Api.Models
{
    public class Workout
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime CurrentDate { get; set; }

        public string ActivityType { get; set; } = string.Empty;

        public int Duration { get; set; }

        public int Sets { get; set; }

        public int Reps { get; set; }

        public double Weight { get; set; }

        public IntensityLevel IntensityLevel { get; set; }

        public int CaloriesBurned { get; set; }
    }
}
