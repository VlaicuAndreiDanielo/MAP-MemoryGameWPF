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
        private string _selectedModule = "flori"; // default
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
            set
            {
                _selectedLevel = value;
                OnPropertyChanged(nameof(SelectedLevel));
                OnPropertyChanged(nameof(IsSizeEditable));
                OnPropertyChanged(nameof(IsTimeEditable));  // notificăm schimbarea timpului

                // Setează automat valorile pentru rânduri, coloane și timp în funcție de nivel
                switch (value.ToLower())
                {
                    case "baby mode":
                        Rows = 2;
                        Columns = 2;
                        Time = 30; // secunde
                        break;
                    case "easy":
                        // Random între 2x3 și 3x2
                        if (new Random().Next(2) == 0)
                        {
                            Rows = 2;
                            Columns = 3;
                        }
                        else
                        {
                            Rows = 3;
                            Columns = 2;
                        }
                        Time = 45;
                        break;
                    case "medium":
                        // Random între 3x4, 4x3 și 4x4
                        int m = new Random().Next(3);
                        if (m == 0)
                        {
                            Rows = 3;
                            Columns = 4;
                        }
                        else if (m == 1)
                        {
                            Rows = 4;
                            Columns = 3;
                        }
                        else
                        {
                            Rows = 4;
                            Columns = 4;
                        }
                        Time = 50;
                        break;
                    case "hard":
                        // Random între 5x6 și 6x5
                        if (new Random().Next(2) == 0)
                        {
                            Rows = 5;
                            Columns = 6;
                        }
                        else
                        {
                            Rows = 6;
                            Columns = 5;
                        }
                        Time = 60;
                        break;
                    case "very hard":
                        Rows = 6;
                        Columns = 6;
                        Time = 80; // 1 min 20 sec
                        break;
                    case "intermediate":
                        // Random între 4x5 și 5x4
                        if (new Random().Next(2) == 0)
                        {
                            Rows = 4;
                            Columns = 5;
                        }
                        else
                        {
                            Rows = 5;
                            Columns = 4;
                        }
                        Time = 55;
                        break;
                    case "challenging":
                        Rows = 6;
                        Columns = 6;
                        Time = 60;
                        break;
                    case "expert":
                        // Random între 6x7 și 7x6
                        if (new Random().Next(2) == 0)
                        {
                            Rows = 6;
                            Columns = 7;
                        }
                        else
                        {
                            Rows = 7;
                            Columns = 6;
                        }
                        Time = 60;
                        break;
                    case "nightmare":
                        // Random între 7x8 și 8x7
                        if (new Random().Next(2) == 0)
                        {
                            Rows = 7;
                            Columns = 8;
                        }
                        else
                        {
                            Rows = 8;
                            Columns = 7;
                        }
                        Time = 120; // 2 min
                        break;
                    case "hell mode":
                        // Random între 8x9, 9x8 și 8x8
                        int h = new Random().Next(3);
                        if (h == 0)
                        {
                            Rows = 8;
                            Columns = 9;
                        }
                        else if (h == 1)
                        {
                            Rows = 9;
                            Columns = 8;
                        }
                        else
                        {
                            Rows = 8;
                            Columns = 8;
                        }
                        Time = 150; // 2 min 30 sec
                        break;
                    case "insane":
                        // Random între 9x10 și 10x9
                        if (new Random().Next(2) == 0)
                        {
                            Rows = 9;
                            Columns = 10;
                        }
                        else
                        {
                            Rows = 10;
                            Columns = 9;
                        }
                        Time = 210; // 3 min 30 sec
                        break;
                    case "god mode":
                        Rows = 10;
                        Columns = 10;
                        Time = 300; // 5 min
                        break;
                    case "custom":
                        // Permite editarea
                        break;
                    default:
                        break;
                }
                OnPropertyChanged(nameof(Rows));
                OnPropertyChanged(nameof(Columns));
                OnPropertyChanged(nameof(Time));
            }
        }

        public bool IsSizeEditable => SelectedLevel.ToLower() == "custom";
        public bool IsTimeEditable => SelectedLevel.ToLower() == "custom";

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
            var newGame = new Game
            {
                Level = SelectedLevel,
                Module = SelectedModule, // De exemplu: "flori", "animale", etc.
                Rows = Rows,
                Columns = Columns,
                TimeRemaining = Time,
                Cards = new List<CardState>()
            };

            var gameView = new GameView();
            var gameVM = new GameViewModel(_currentUser, newGame);
            gameView.DataContext = gameVM;
            gameView.Show();

            if (parameter is Window window)
                window.Close();

            foreach (Window w in Application.Current.Windows)
            {
                if (w is LoginView)
                {
                    w.Close();
                    break;
                }
            }
        }

        private void GoBack(object parameter)
        {
            if (parameter is Window window)
                window.Close();
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
                window.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
