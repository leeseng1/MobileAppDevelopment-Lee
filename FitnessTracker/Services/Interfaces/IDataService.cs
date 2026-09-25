using FitnessTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FitnessTracker.Services.Interfaces
{
    public interface IDataService
    {
        Task<IEnumerable<WorkoutEntry>> GetWorkoutEntriesAsync();
        Task SaveWorkoutAsync(WorkoutEntry workout);

        Task<IEnumerable<FoodEntry>> GetFoodEntriesAsync();
        Task SaveFoodEntryAsync(FoodEntry foodEntry);

        Task<IEnumerable<WorkoutGoalEntry>> GetWorkoutGoalsAsync();
        Task<DietGoalEntry> GetDietGoalAsync();
    }
}
