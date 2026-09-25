using System;
using System.Collections.Generic;
using System.Text;

namespace FitnessTracker.Models
{
    public class DietGoalEntry
    {
        public int Id { get; set; }

        public DateTime StartDate { get; set; } = DateTime.Now;

        public double TargetCalories { get; set; }

        public double TargetProtein { get; set; }

        public double TargetCarbs { get; set; }

        public double TargetFats { get; set; }

        public int TargetMealsPerDay { get; set; }

    }
}
