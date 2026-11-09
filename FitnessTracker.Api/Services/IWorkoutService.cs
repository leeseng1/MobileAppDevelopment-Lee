using FitnessTracker.Api.DTOs;

namespace FitnessTracker.Api.Services;

public interface IWorkoutService
{
    Task<IReadOnlyList<WorkoutResponse>> GetAllAsync(string? type, CancellationToken ct);
    Task<WorkoutResponse?> GetByIdAsync(int id, CancellationToken ct);
    Task<WorkoutResult> CreateAsync(WorkoutRequest request, CancellationToken ct);
    Task<WorkoutResult> UpdateAsync(int id, WorkoutRequest request, CancellationToken ct);
    Task<WorkoutOutcome> DeleteAsync(int id, CancellationToken ct);
}