using MemoryGame.Commands;
using MemoryGame.Models;
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
            RetryCommand = new RelayCommand(Retry);
            RestartCommand = new RelayCommand(Restart);
            QuitCommand = new RelayCommand(Quit);
        }

        public ICommand RetryCommand { get; }
        public ICommand RestartCommand { get; }
        public ICommand QuitCommand { get; }

        // Retry recreează jocul cu aceleași setări
        private void Retry(object parameter)
        {
            var gameView = new Views.GameView();
            var gameVM = new GameViewModel(_currentUser, _currentGame);
            gameView.DataContext = gameVM;
            gameView.Show();

            if (parameter is Window window)
                window.Close();
        }

        // Restart deschide interfața de Game Setup pentru reconfigurare
        private void Restart(object parameter)
        {
            var setupView = new Views.GameSetupView();
            var setupVM = new GameSetupViewModel(_currentUser);
            setupView.DataContext = setupVM;
            setupView.Show();

            if (parameter is Window window)
                window.Close();
        }

        // Quit revine la ecranul de Login
        private void Quit(object parameter)
        {
            var loginView = new Views.LoginView();
            loginView.Show();

            // Închide GameOver și fereastra de joc, dacă e cazul
            if (parameter is Window window)
                window.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
