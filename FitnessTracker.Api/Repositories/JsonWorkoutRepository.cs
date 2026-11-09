using System.Text.Json;
using System.Text.Json.Serialization;
using FitnessTracker.Api.Models;

namespace FitnessTracker.Api.Repositories;

/// <summary>
/// Provides a JSON file-based repository for workout data.
/// </summary>
/// <remarks>
/// Stores workout data in the application's Data directory and 
/// synchronizes file access for concurrent operations.
/// </remarks>
public class JsonWorkoutRepository : IWorkoutRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };


      
    private readonly string _filePath;

    // Semaphore to ensure thread-safe access to the JSON file
    // This is necessary because multiple requests could be trying to read/write the file at the same time.
    private readonly SemaphoreSlim _lock = new(1, 1);

    public JsonWorkoutRepository(IWebHostEnvironment env)
    {
        var dataDirectory = Path.Combine(env.ContentRootPath, "Data");
        Directory.CreateDirectory(dataDirectory);
        _filePath = Path.Combine(dataDirectory, "workouts.json");
    }

    public async Task<IReadOnlyList<Workout>> GetAllAsync(CancellationToken ct)
    {
        // Ensure thread-safe access to the JSON file by acquiring the semaphore lock before reading.
        await _lock.WaitAsync(ct);
        try
        {
            return await ReadAllAsync(ct);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Workout?> GetByIdAsync(int id, CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var workouts = await ReadAllAsync(ct);
            return workouts.FirstOrDefault(w => w.Id == id);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Workout> AddAsync(Workout workout, CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var workouts = await ReadAllAsync(ct);

            workout.Id = workouts.Count == 0 ? 1 : workouts.Max(w => w.Id) + 1;
            workout.CreatedAt = DateTime.UtcNow;

            workouts.Add(workout);
            await WriteAllAsync(workouts, ct);
            return workout;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> UpdateAsync(Workout workout, CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var workouts = await ReadAllAsync(ct);
            var index = workouts.FindIndex(w => w.Id == workout.Id);
            if (index < 0)
            {
                return false;
            }

            workout.CreatedAt = workouts[index].CreatedAt; // client can't change this
            workouts[index] = workout;
            await WriteAllAsync(workouts, ct);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        await _lock.WaitAsync(ct);
        try
        {
            var workouts = await ReadAllAsync(ct);
            if (workouts.RemoveAll(w => w.Id == id) == 0)
            {
                return false;
            }

            await WriteAllAsync(workouts, ct);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    // Private helpers: only called from inside a guarded section, so they do NOT take the lock.
    private async Task<List<Workout>> ReadAllAsync(CancellationToken ct)
    {
        if (!File.Exists(_filePath))
        {
            return new List<Workout>();
        }

        var json = await File.ReadAllTextAsync(_filePath, ct);
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<Workout>();
        }

        return JsonSerializer.Deserialize<List<Workout>>(json, JsonOptions) ?? new List<Workout>();
    }

    // Private helpers: only called from inside a guarded section, so they do NOT take the lock.
    private async Task WriteAllAsync(List<Workout> workouts, CancellationToken ct)
    {
        // Write to a temporary file first, then move it to the target location.
        var tempPath = _filePath + ".tmp";
        var json = JsonSerializer.Serialize(workouts, JsonOptions);

        await File.WriteAllTextAsync(tempPath, json, ct);
        File.Move(tempPath, _filePath, overwrite: true);
    }
}