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

        public bool ResumeOnClose { get; set; } = true;

        public void ContinueGame(object parameter)
        {
            ResumeOnClose = true;
            if (parameter is Window window)
            {
                window.Close();
            }
            _gameVM.ResumeTimer();
            _gameVM.IsForcedQuit = true;
        }

        private void SaveGame(object parameter)
        {
            ResumeOnClose = false;
            Game currentGameState = _gameVM.GetCurrentGameState();
            _currentUser.SavedGame = currentGameState;
            _currentUser.SavedGame.IsSaved = true;
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
            var loginView = new Views.LoginView();
            loginView.Show();

            if (parameter is Window window)
            {
                window.Close();
            }
            _gameVM.PauseTimer(); 
            foreach (Window w in Application.Current.Windows)
            {
                if (w is Views.GameView || w is Views.PauseWindow)
                {
                    w.Close();
                }
            }
        }

        private void QuitGame(object parameter)
        {
            ResumeOnClose = false;
            _gameVM.PauseTimer();

            var loginView = new Views.LoginView();
            loginView.Show();

            foreach (Window w in Application.Current.Windows)
            {
                if (w is Views.GameView || w is Views.PauseWindow)
                {
                    w.Close();
                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
