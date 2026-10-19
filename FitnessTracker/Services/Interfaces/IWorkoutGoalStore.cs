using FitnessTracker.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FitnessTracker.Services.Interfaces
{
    public interface IWorkoutGoalStore
    {
        Task<IReadOnlyList<WorkoutGoalEntry>> GetWorkoutGoalsAsync();

        Task AddWorkoutGoalAsync(WorkoutGoalEntry goal);

        Task UpdateWorkoutGoalAsync(WorkoutGoalEntry goal);
    }
}
