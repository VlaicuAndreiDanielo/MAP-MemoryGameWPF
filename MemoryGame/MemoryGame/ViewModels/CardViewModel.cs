using System.ComponentModel;
using System.Windows.Input;

namespace MemoryGame.Models
{
    public class CardViewModel : INotifyPropertyChanged
    {
        private static readonly Random _rnd = new Random();

        private static readonly string[] DefaultFrontImages =
        {
            "../ImagesCards/default_card_front.png",
            "../ImagesCards/default_card_front1.png",
            "../ImagesCards/default_card_front2.png",
            "../ImagesCards/default_card_front3.png"
        };
        private static string _gameFrontImage = null;
  
        public string FrontImage { get; }
        public string BackImage { get; }
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
        public int Index { get; set; }
        public int Columns { get; }
        public int Row => Index / Columns;
        public int Column => Index % Columns;

        public bool ShowFront => IsFlipped || IsMatched;

        public CardViewModel(string backImage, int columns)
        {
            BackImage = "../" + backImage;
            IsFlipped = false;
            IsMatched = false;
            Columns = columns;
            if (_gameFrontImage == null)
            {
                _gameFrontImage = DefaultFrontImages[_rnd.Next(DefaultFrontImages.Length)];
            }
            FrontImage = _gameFrontImage;
        }

        public void Flip()
        {
            if (IsMatched)
                return; 
            IsFlipped = !IsFlipped;
            OnPropertyChanged(nameof(IsFlipped));
            OnPropertyChanged(nameof(ShowFront));
        }

        public void SetMatched(bool matched)
        {
            IsMatched = matched;
            OnPropertyChanged(nameof(IsMatched));
            OnPropertyChanged(nameof(ShowFront));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
