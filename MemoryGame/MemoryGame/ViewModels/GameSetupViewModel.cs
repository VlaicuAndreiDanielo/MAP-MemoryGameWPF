using MemoryGame.Commands;
using MemoryGame.Models;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using MemoryGame.Views;

namespace MemoryGame.ViewModels
{
    public class GameSetupViewModel : INotifyPropertyChanged
    {
        private User _currentUser;
        private string _selectedLevel = "easy"; // default
        private string _selectedModule = "animale"; // default
        private int _rows = 4;
        private int _columns = 4;
        private int _time = 60;

        public GameSetupViewModel(User user)
        {
            _currentUser = user;

            PlayCommand = new RelayCommand(PlayGame);
            BackCommand = new RelayCommand(GoBack);
            ContinueCommand = new RelayCommand(ContinueGame, CanContinueGame);
        }

        public string SelectedLevel
        {
            get => _selectedLevel;
            set { _selectedLevel = value; OnPropertyChanged(nameof(SelectedLevel)); }
        }

        public string SelectedModule
        {
            get => _selectedModule;
            set { _selectedModule = value; OnPropertyChanged(nameof(SelectedModule)); }
        }

        public int Rows
        {
            get => _rows;
            set { _rows = value; OnPropertyChanged(nameof(Rows)); }
        }

        public int Columns
        {
            get => _columns;
            set { _columns = value; OnPropertyChanged(nameof(Columns)); }
        }

        public int Time
        {
            get => _time;
            set { _time = value; OnPropertyChanged(nameof(Time)); }
        }

        public ICommand PlayCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand ContinueCommand { get; }

        private void PlayGame(object parameter)
        {
            // Inițializează un nou Game
            var newGame = new Game
            {
                Level = SelectedLevel,
                Module = SelectedModule,
                Rows = Rows,
                Columns = Columns,
                TimeRemaining = Time,
                // Cards se vor genera la momentul pornirii jocului
            };

            // Deschide GameView cu noul Game
            var gameView = new GameView();
            var gameVM = new GameViewModel(_currentUser, newGame);
            gameView.DataContext = gameVM;
            gameView.Show();

            // Închide fereastra curentă (GameSetupView)
            if (parameter is Window window)
            {
                window.Close();
            }
        }

        private void GoBack(object parameter)
        {
            // Revine la LoginView (sau închide fereastra)
            if (parameter is Window window)
            {
                window.Close();
            }
        }

        private bool CanContinueGame(object parameter)
        {
            return _currentUser?.SavedGame != null;
        }

        private void ContinueGame(object parameter)
        {
            if (_currentUser?.SavedGame == null) return;

            var gameView = new GameView();
            var gameVM = new GameViewModel(_currentUser, _currentUser.SavedGame);
            gameView.DataContext = gameVM;
            gameView.Show();

            if (parameter is Window window)
            {
                window.Close();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
