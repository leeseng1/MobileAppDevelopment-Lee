using System.ComponentModel;

namespace FitnessTracker.Models
{
    public class FoodEntry : INotifyPropertyChanged
    {
        private bool isDetailsExpanded;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public MealType Meal { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Calories { get; set; }

        public double Protein { get; set; }

        public double Carbs { get; set; }

        public double Fats { get; set; }

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
