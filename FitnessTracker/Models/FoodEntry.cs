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

        /// <summary>
        /// Gets or sets a value indicating whether the details section is expanded.
        /// </summary>
        /// <remarks>Setting the property raises PropertyChanged. Assigning the current value has no
        /// effect.</remarks>
        public bool IsDetailsExpanded
        {
            get => isDetailsExpanded;
            set
            {
                if (isDetailsExpanded == value)
                {
                    return;
                }
                isDetailsExpanded = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDetailsExpanded)));
            }
        }

    }

    public enum MealType
    {
        Breakfast,
        Lunch,
        Dinner,
        Snack
    }

}
