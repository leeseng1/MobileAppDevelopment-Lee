using FitnessTracker.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FitnessTracker.Data
{
    public class FitnessTrackerDbContext(DbContextOptions<FitnessTrackerDbContext> options) : DbContext(options)
    {
        public DbSet<WorkoutEntry> WorkoutEntries => Set<WorkoutEntry>();

        public DbSet<FoodEntry> FoodEntries => Set<FoodEntry>();

        public DbSet<WorkoutGoalEntry> WorkoutGoals => Set<WorkoutGoalEntry>();

        public DbSet<DietGoalEntry> DietGoals => Set<DietGoalEntry>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WorkoutGoalEntry>().Ignore(goal => goal.IsAchieved);
        }
    }
}
