using FitnessTracker.Data;
using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FitnessTracker.Services
{
    public sealed class WorkoutGoalStore(IDbContextFactory<FitnessTrackerDbContext> contextFactory) : IWorkoutGoalStore
    {
        // Retrieves all workout goals from the database, ordered by their ID.
        public async Task<IReadOnlyList<WorkoutGoalEntry>> GetWorkoutGoalsAsync()
        {
            await using var context = await contextFactory.CreateDbContextAsync();

            return await context.WorkoutGoals.AsNoTracking().OrderBy(goal => goal.Id).ToListAsync();
        }

        // Retrieves a specific workout goal by its ID. Returns null if not found.
        public async Task AddWorkoutGoalAsync(WorkoutGoalEntry goal)
        {
            ArgumentNullException.ThrowIfNull(goal);

            await using var context = await contextFactory.CreateDbContextAsync();

            context.WorkoutGoals.Add(goal);
            await context.SaveChangesAsync();
        }

        // Updates an existing workout goal in the database. Throws an exception if the goal does not exist.
        public async Task UpdateWorkoutGoalAsync(WorkoutGoalEntry goal)
        {
            ArgumentNullException.ThrowIfNull(goal);

            await using var context = await contextFactory.CreateDbContextAsync();

            var storedGoal = await context.WorkoutGoals.SingleOrDefaultAsync(item => item.Id == goal.Id);

            if (storedGoal is null)
            {
                throw new InvalidOperationException($"Workout goal with ID {goal.Id} not found");
            }

            storedGoal.Title = goal.Title;
            storedGoal.Type = goal.Type;
            storedGoal.Unit = goal.Unit;
            storedGoal.StartingValue = goal.StartingValue;
            storedGoal.TargetValue = goal.TargetValue;
            storedGoal.CurrentValue = goal.CurrentValue;
            storedGoal.StartDate = goal.StartDate;
            storedGoal.EndDate = goal.EndDate;

            await context.SaveChangesAsync();
        }
    }
}
