using FitnessTracker.Api.DTOs;
using FitnessTracker.Api.Models;
using FitnessTracker.Api.Repositories;

namespace FitnessTracker.Api.Services;

public class WorkoutService : IWorkoutService
{
    private readonly IWorkoutRepository _repository;

    public WorkoutService(IWorkoutRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<WorkoutResponse>> GetAllAsync(string? type, CancellationToken ct)
    {
        var workouts = await _repository.GetAllAsync(ct);

        IEnumerable<Workout> query = workouts;
        if (!string.IsNullOrWhiteSpace(type))
        {
            query = query.Where(w =>
                string.Equals(w.ActivityType, type.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return query.Select(ToResponse).ToList();
    }

    public async Task<WorkoutResponse?> GetByIdAsync(int id, CancellationToken ct)
    {
        var workout = await _repository.GetByIdAsync(id, ct);
        return workout is null ? null : ToResponse(workout);
    }

    public async Task<WorkoutResult> CreateAsync(WorkoutRequest request, CancellationToken ct)
    {
        var workout = ToModel(request);

        var existing = await _repository.GetAllAsync(ct);
        if (IsDuplicate(existing, workout, excludeId: null))
        {
            return new WorkoutResult(WorkoutOutcome.Duplicate);
        }

        var created = await _repository.AddAsync(workout, ct);
        return new WorkoutResult(WorkoutOutcome.Success, ToResponse(created));
    }

    public async Task<WorkoutResult> UpdateAsync(int id, WorkoutRequest request, CancellationToken ct)
    {
        var current = await _repository.GetByIdAsync(id, ct);
        if (current is null)
        {
            return new WorkoutResult(WorkoutOutcome.NotFound);
        }

        var workout = ToModel(request);
        workout.Id = id;

        var existing = await _repository.GetAllAsync(ct);
        if (IsDuplicate(existing, workout, excludeId: id))
        {
            return new WorkoutResult(WorkoutOutcome.Duplicate);
        }

        var updated = await _repository.UpdateAsync(workout, ct);
        if (!updated)
        {
            // Deleted by someone else between our check and the update.
            return new WorkoutResult(WorkoutOutcome.NotFound);
        }

        // The repository restores CreatedAt on the object we passed in.
        return new WorkoutResult(WorkoutOutcome.Success, ToResponse(workout));
    }

    public async Task<WorkoutOutcome> DeleteAsync(int id, CancellationToken ct)
    {
        var deleted = await _repository.DeleteAsync(id, ct);
        return deleted ? WorkoutOutcome.Success : WorkoutOutcome.NotFound;
    }

    // The business rule: same activity type on the same day is a duplicate.
    private static bool IsDuplicate(IEnumerable<Workout> existing, Workout candidate, int? excludeId)
    {
        return existing.Any(w =>
            w.Id != excludeId &&
            string.Equals(w.ActivityType, candidate.ActivityType, StringComparison.OrdinalIgnoreCase) &&
            w.CurrentDate.Date == candidate.CurrentDate.Date);
    }

    private static Workout ToModel(WorkoutRequest request)
    {
        return new Workout
        {
            // Validation guarantees CurrentDate is present before the service is called.
            CurrentDate = request.CurrentDate!.Value,
            ActivityType = request.ActivityType.Trim(),
            Duration = request.Duration,
            Sets = request.Sets,
            Reps = request.Reps,
            Weight = request.Weight,
            IntensityLevel = request.IntensityLevel,
            CaloriesBurned = request.CaloriesBurned
        };
    }

    private static WorkoutResponse ToResponse(Workout workout)
    {
        return new WorkoutResponse
        {
            Id = workout.Id,
            CreatedAt = workout.CreatedAt,
            CurrentDate = workout.CurrentDate,
            ActivityType = workout.ActivityType,
            Duration = workout.Duration,
            Sets = workout.Sets,
            Reps = workout.Reps,
            Weight = workout.Weight,
            IntensityLevel = workout.IntensityLevel,
            CaloriesBurned = workout.CaloriesBurned
        };
    }
}