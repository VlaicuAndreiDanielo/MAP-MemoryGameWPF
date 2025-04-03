using MemoryGame.Models;
using MemoryGame.Services;
using MemoryGame.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;

namespace MemoryGame.ViewModels
{
    public class StatisticsViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<UserStats> _allStats;
        private ObservableCollection<object> _displayStats;
        public ObservableCollection<object> DisplayStats
        {
            get => _displayStats;
            set { _displayStats = value; OnPropertyChanged(nameof(DisplayStats)); }
        }

        public ICommand ShowOverallCommand { get; }
        public ICommand ShowCategoryCommand { get; }
        public ICommand ShowModeCommand { get; }

        public StatisticsViewModel()
        {
            // Încărcăm statisticile din fișierul JSON
            _allStats = new ObservableCollection<UserStats>(StatsManager.LoadStats());
            ShowOverallCommand = new RelayCommand(o => ShowOverall());
            ShowCategoryCommand = new RelayCommand(o => ShowCategory());
            ShowModeCommand = new RelayCommand(o => ShowMode());
            ShowOverall();
        }

        private void ShowOverall()
        {
            // Afișează statisticile generale pentru fiecare utilizator
            DisplayStats = new ObservableCollection<object>(_allStats.Select(s => new {
                s.UserName,
                s.TotalGames,
                s.Wins,
                s.Losses
            }));
        }

        private void ShowCategory()
        {
            // Afișează statisticile pe categorii pentru fiecare utilizator
            var list = new List<object>();
            foreach (var s in _allStats)
            {
                foreach (var cat in s.Categories)
                {
                    list.Add(new { s.UserName, Category = cat.Key, Wins = cat.Value.Wins, Losses = cat.Value.Losses });
                }
            }
            DisplayStats = new ObservableCollection<object>(list);
        }

        private void ShowMode()
        {
            // Afișează statisticile pe moduri (dificultăți) pentru fiecare utilizator
            var list = new List<object>();
            foreach (var s in _allStats)
            {
                foreach (var diff in s.Difficulties)
                {
                    list.Add(new { s.UserName, Difficulty = diff.Key, Wins = diff.Value.Wins, Losses = diff.Value.Losses });
                }
            }
            DisplayStats = new ObservableCollection<object>(list);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
           => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
