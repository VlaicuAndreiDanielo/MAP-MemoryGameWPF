using MemoryGame.Models;
using MemoryGame.Commands;
using System.ComponentModel;
using System.Windows.Input;
using MemoryGame.Services; // pentru UserManager
using System.Windows;
using MemoryGame.Views;

namespace MemoryGame.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        private User _currentUser;
        private Game _currentGame;

        public GameViewModel(User user, Game game)
        {
            _currentUser = user;
            _currentGame = game;

            PauseCommand = new RelayCommand(ShowPauseMenu);
        }

        public GameViewModel()
        {
            // ...
        }

        public ICommand PauseCommand { get; }

        private void ShowPauseMenu(object parameter)
        {
            // Deschide un pop-up (fereastră) care are butoane: Continue, Save, Quit
            var pauseWindow = new PauseWindow(); // o fereastră mică
            var pauseVM = new PauseViewModel(_currentUser, _currentGame);
            pauseWindow.DataContext = pauseVM;
            pauseWindow.ShowDialog();
        }

        // Aici vei avea logica de joc, flipping cards, timer etc.

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
