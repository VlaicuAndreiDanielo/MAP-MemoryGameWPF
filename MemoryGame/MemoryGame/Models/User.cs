using System.ComponentModel;

namespace MemoryGame.Models
{
    public class User : INotifyPropertyChanged
    {
        private string _name;
        private string _imagePath;
        private Game _savedGame;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string ImagePath
        {
            get => _imagePath;
            set
            {
                _imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        public Game SavedGame
        {
            get => _savedGame;
            set
            {
                _savedGame = value;
                OnPropertyChanged(nameof(SavedGame));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
