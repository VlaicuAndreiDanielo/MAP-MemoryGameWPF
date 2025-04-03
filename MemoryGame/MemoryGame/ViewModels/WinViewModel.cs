using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MemoryGame.ViewModels
{
    public class WinViewModel : INotifyPropertyChanged
    {
        private User _currentUser;
        private Game _currentGame;

        public WinViewModel(User user, Game game)
        {
            _currentUser = user;
            _currentGame = game;

            // Actualizează statistica de câștig pentru dificultate și categorie
            StatsManager.UpdateStats(_currentUser, _currentGame, win: true);

            ReplayCommand = new RelayCommand(Replay);
            HomeCommand = new RelayCommand(Home);
            QuitCommand = new RelayCommand(Quit);
        }

        public ICommand ReplayCommand { get; }
        public ICommand HomeCommand { get; }
        public ICommand QuitCommand { get; }

        private void Replay(object parameter)
        {
            // Pornește un nou joc cu aceleași setări, dar o configurație diferită
            var gameView = new Views.GameView();
            var gameVM = new GameViewModel(_currentUser, _currentGame); // folosim același Game ca șablon
            gameView.DataContext = gameVM;
            gameView.Show();

            if (parameter is Window window)
                window.Close();
        }

        private void Home(object parameter)
        {
            // Deschide fereastra de Game Setup pentru reconfigurare
            var setupView = new Views.GameSetupView();
            var setupVM = new GameSetupViewModel(_currentUser);
            setupView.DataContext = setupVM;
            setupView.Show();

            if (parameter is Window window)
                window.Close();
        }

        private void Quit(object parameter)
        {
            // Revine la fereastra de Login
            var loginView = new Views.LoginView();
            loginView.Show();

            if (parameter is Window window)
                window.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
