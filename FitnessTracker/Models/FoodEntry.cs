namespace FitnessTracker.Models
{
    public class FoodEntry
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public MealType Meal { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Calories { get; set; }

        public double Protein { get; set; }

        public double Carbs { get; set; }

        public double Fats { get; set; }
    }

    public enum MealType
    {
        Breakfast,
        Lunch,
        Dinner,
        Snack
    }

}
