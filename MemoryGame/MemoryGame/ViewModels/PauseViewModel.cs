using MemoryGame.Commands;
using MemoryGame.Models;
using MemoryGame.Services;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MemoryGame.ViewModels
{
    public class PauseViewModel : INotifyPropertyChanged
    {
        private User _currentUser;
        private GameViewModel _gameVM;

        public PauseViewModel(User user, GameViewModel gameVM)
        {
            _currentUser = user;
            _gameVM = gameVM;
            ContinueCommand = new RelayCommand(ContinueGame);
            SaveCommand = new RelayCommand(SaveGame);
            QuitCommand = new RelayCommand(QuitGame);
        }

        public ICommand ContinueCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand QuitCommand { get; }

        private void ContinueGame(object parameter)
        {
            if (parameter is Window window)
            {
                window.Close();
            }
            _gameVM.ResumeTimer();
        }

        private void SaveGame(object parameter)
        {
            // Actualizează starea jocului curent
            Game currentGameState = _gameVM.GetCurrentGameState();
            _currentUser.SavedGame = currentGameState;
            var users = UserManager.LoadUsers();
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Name == _currentUser.Name)
                {
                    users[i].SavedGame = currentGameState;
                    break;
                }
            }
            UserManager.SaveUsers(users);

            if (parameter is Window window)
            {
                window.Close();
            }
            _gameVM.ResumeTimer();

            foreach (Window w in Application.Current.Windows)
            {
                if (w is Views.GameView || w is Views.PauseWindow)
                {
                    w.Close();
                }
            }
            var loginView = new Views.LoginView();
            loginView.Show();
        }

        private void QuitGame(object parameter)
        {
            // Închide fereastra de pauză și jocul, apoi revine la Login
            foreach (Window w in Application.Current.Windows)
            {
                if (w is Views.GameView || w is Views.PauseWindow)
                {
                    w.Close();
                }
            }

            Window loginView = new Views.LoginView();
            loginView.Show();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
