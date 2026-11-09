using System.ComponentModel.DataAnnotations;
using FitnessTracker.Api.Models;

namespace FitnessTracker.Api.DTOs
{
    public class WorkoutRequest
    {
        [Required(ErrorMessage = "Date is required.")]
        public DateTime? CurrentDate { get; set; }

        [Required(ErrorMessage = "Activity type is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Activity type must be between 2 and 50 characters.")]
        public string ActivityType { get; set; } = string.Empty;

        [Range(1, 1440, ErrorMessage = "Duration must be between 1 and 1440 minutes.")]
        public int Duration { get; set; }

        [Range(0, 100, ErrorMessage = "Sets must be between 0 and 100.")]
        public int Sets { get; set; }

        [Range(0, 1000, ErrorMessage = "Reps must be between 0 and 1000.")]
        public int Reps { get; set; }

        [Range(0, 2000, ErrorMessage = "Weight must be between 0 and 2000.")]
        public double Weight { get; set; }

        [EnumDataType(typeof(IntensityLevel), ErrorMessage = "Intensity level must be Low, Medium, or High.")]
        public IntensityLevel IntensityLevel { get; set; }

        [Range(0, 20000, ErrorMessage = "Calories burned must be between 0 and 20000.")]
        public int CaloriesBurned { get; set; }
    }
}
