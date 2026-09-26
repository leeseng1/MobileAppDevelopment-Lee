using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FitnessTracker.Services
{
    public class MockDataService : IDataService
    {
        private List<WorkoutEntry> workouts;

        private List<FoodEntry> foodEntries;

        private List<WorkoutGoalEntry> workoutGoals;

        private DietGoalEntry dietGoal;

        public MockDataService()
        {
            workouts = new List<WorkoutEntry>
            {
                new WorkoutEntry { Id = 1, Date = DateTime.Now.AddDays(-1), ActivityType = "Running", Duration = 30, CaloriesBurned = 300 },
                new WorkoutEntry {Id = 2, Date = DateTime.Now.AddDays(-2), ActivityType = "Chest Exercise", Duration = 25, CaloriesBurned = 200, Reps = 10, Weight = 100 }
            };

            foodEntries = new List<FoodEntry>
            {
                new FoodEntry { Id = 1, Date = DateTime.Now.AddDays(-1), Name = "Apple", Calories = 95, Protein = 0.5, Carbs = 25, Fats = 0.3 },
                new FoodEntry { Id = 2, Date = DateTime.Now.AddDays(-2), Name = "Chicken Breast", Calories = 165, Protein = 31, Carbs = 0, Fats = 3.6 }
            };

            workoutGoals = new List<WorkoutGoalEntry>
            {
                new WorkoutGoalEntry { Id = 1, Title = "Lose Weight", Type = GoalType.WeightLoss, TargetValue = 150, CurrentValue = 160, StartDate = DateTime.Now.AddDays(-30), EndDate = DateTime.Now.AddDays(30) }
            };

            dietGoal = new DietGoalEntry
            {
                Id = 1,
                StartDate = DateTime.Now.AddDays(-7),
                TargetCalories = 2000,
                TargetProtein = 150,
                TargetCarbs = 250,
                TargetFats = 70,
                TargetMealsPerDay = 3
            };
        }

        public async Task<IEnumerable<WorkoutEntry>> GetWorkoutEntriesAsync()
        {
            await Task.Delay(300);
            return workouts;
        }

        public async Task SaveWorkoutAsync(WorkoutEntry workout)
        {
            await Task.Delay(300);
            workouts.Add(workout);
        }

        public async Task<IEnumerable<FoodEntry>> GetFoodEntriesAsync()
        {
            await Task.Delay(300);
            return foodEntries;
        }

        public async Task SaveFoodEntryAsync(FoodEntry foodEntry)
        {
            await Task.Delay(300);
            foodEntries.Add(foodEntry);
        }

        public async Task<IEnumerable<WorkoutGoalEntry>> GetWorkoutGoalsAsync()
        {
            await Task.Delay(300);
            return workoutGoals;
        }

        public async Task<DietGoalEntry> GetDietGoalAsync()
        {
            await Task.Delay(300);
            return dietGoal;
        }

        public async Task SaveWorkoutGoalAsync(WorkoutGoalEntry goal)
        {
            await Task.Delay(300);
            workoutGoals.Add(goal);
        }

        public async Task SaveDietGoalAsync(DietGoalEntry goal)
        {
            await Task.Delay(300);
            dietGoal = goal;
        }
    }
}
