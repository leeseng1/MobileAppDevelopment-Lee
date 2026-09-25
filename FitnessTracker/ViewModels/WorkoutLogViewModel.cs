using FitnessTracker.Models;
using FitnessTracker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;

namespace FitnessTracker.ViewModels
{
    public class WorkoutLogViewModel : INotifyPropertyChanged
    {
        private readonly IDataService dataService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<WorkoutEntry> WorkoutEntries { get; set; }

        public ICommand LoadWorkoutsCommand { get; }

        public WorkoutLogViewModel(IDataService dataService)
        {
            this.dataService = dataService;

            WorkoutEntries = new ObservableCollection<WorkoutEntry>();

            LoadWorkoutsCommand = new Command(async () => await LoadWorkoutsAsync());
        }

        private async Task LoadWorkoutsAsync()
        {
            var workouts = await dataService.GetWorkoutEntriesAsync();
            WorkoutEntries.Clear();
            foreach (var workout in workouts)
            {
                WorkoutEntries.Add(workout);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
