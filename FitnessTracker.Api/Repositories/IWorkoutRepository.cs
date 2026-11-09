using FitnessTracker.Api.Models;

namespace FitnessTracker.Api.Repositories
{
    public interface IWorkoutRepository
    {
        Task<IReadOnlyList<Workout>> GetAllAsync(CancellationToken ct);

        Task<Workout?> GetByIdAsync(int id, CancellationToken ct);

        Task<Workout> AddAsync(Workout workout, CancellationToken ct);

        Task<bool> UpdateAsync(Workout workout, CancellationToken ct);

        Task<bool> DeleteAsync(int id, CancellationToken ct);
    }
}
