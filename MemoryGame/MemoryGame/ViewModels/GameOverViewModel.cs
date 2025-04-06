using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Service;
using MemoryGame.Services;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MemoryGame.ViewModels
{
    public class GameOverViewModel : INotifyPropertyChanged
    {
        private User _currentUser;
        private Game _currentGame;

        public GameOverViewModel(User currentUser, Game currentGame)
        {
            _currentUser = currentUser;
            _currentGame = currentGame;
            StatsManager.UpdateStats(_currentUser, _currentGame, win: false);
            if (_currentGame.IsSaved)
            {
                GameManager.ClearSavedGame(_currentUser);
                // ClearSavedGame(); - Old function call, but still functional
            }
            RetryCommand = new RelayCommand(Retry);
            RestartCommand = new RelayCommand(Restart);
            QuitCommand = new RelayCommand(Quit);
        }

        public ICommand RetryCommand { get; }
        public ICommand RestartCommand { get; }
        public ICommand QuitCommand { get; }

        private void Retry(object parameter)
        {
            if (_currentGame.IsSaved)
                ClearSavedGame();
            var gameView = new Views.GameView();
            _currentGame.Cards = null; _currentGame.TimeRemaining = Settings.Default.LastTime;
            var gameVM = new GameViewModel(_currentUser, _currentGame);
            gameView.DataContext = gameVM;
            gameView.Show();

            if (parameter is Window window)
                window.Close();
        }

        private void Restart(object parameter)
        {
            if (_currentGame.IsSaved)
                ClearSavedGame();
            var setupView = new Views.GameSetupView();
            var setupVM = new GameSetupViewModel(_currentUser);
            setupView.DataContext = setupVM;
            setupView.Show();

            if (parameter is Window window)
                window.Close();
        }

        private void Quit(object parameter)
        {
            if (_currentGame.IsSaved)
                ClearSavedGame();
            var loginView = new Views.LoginView();
            loginView.Show();

            if (parameter is Window window)
                window.Close();
        }
        private void ClearSavedGame()
        {
            _currentUser.SavedGame = null;
            var users = UserManager.LoadUsers();
            foreach (var user in users)
            {
                if (user.Name.Equals(_currentUser.Name, StringComparison.OrdinalIgnoreCase))
                {
                    user.SavedGame = null;
                    break;
                }
            }
            UserManager.SaveUsers(users);
            CommandManager.InvalidateRequerySuggested();
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
