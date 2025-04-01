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
        private Game _currentGame;

        public PauseViewModel(User user, Game game)
        {
            _currentUser = user;
            _currentGame = game;

            ContinueCommand = new RelayCommand(ContinueGame);
            SaveCommand = new RelayCommand(SaveGame);
            QuitCommand = new RelayCommand(QuitGame);
        }
        public PauseViewModel()
        {
           
        }
        public ICommand ContinueCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand QuitCommand { get; }

        private void ContinueGame(object parameter)
        {
            // Închide doar fereastra de pauză
            if (parameter is Window window)
            {
                window.Close();
            }
        }

        private void SaveGame(object parameter)
        {
            // Salvează starea actuală a jocului în _currentUser.SavedGame
            _currentUser.SavedGame = _currentGame;
            // Actualizează users.json
            var users = UserManager.LoadUsers();
            // Găsește userul actual în listă, actualizează-l
            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Name == _currentUser.Name)
                {
                    users[i].SavedGame = _currentGame;
                    break;
                }
            }
            UserManager.SaveUsers(users);

            // Închide fereastra de pauză
            if (parameter is Window window)
            {
                window.Close();
            }
        }

        private void QuitGame(object parameter)
        {
            // Închide fereastra de pauză și, eventual, fereastra de joc principal
            // Aici decizi tu cum să te întorci la Login sau la alt ecran
            if (parameter is Window window)
            {
                window.Close();
            }

            // Poți să mai cauți fereastra principală de joc și să o închizi
            // ...
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
