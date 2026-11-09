using FitnessTracker.Api.DTOs;

namespace FitnessTracker.Api.Services
{
    public enum WorkoutOutcome
    {
        Success,
        NotFound,
        Duplicate
    }

    public record WorkoutResult(WorkoutOutcome Outcome, WorkoutResponse? Workout = null);
}
