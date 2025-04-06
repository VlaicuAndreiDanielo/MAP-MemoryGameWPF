using System.ComponentModel;
using System.Windows.Input;

namespace MemoryGame.Models
{
    public class CardViewModel : INotifyPropertyChanged
    {
        public string FrontImage { get; }
        public bool IsFlipped { get; set; }
        public bool IsMatched { get; set; }
        public int Index { get; set; }
        public int Columns { get; }
        public int Row => Index / Columns;
        public int Column => Index % Columns;

        public bool ShowFront => IsFlipped || IsMatched;

        public CardViewModel(string frontImage, int columns)
        {
            FrontImage = "../" + frontImage;
            IsFlipped = false;
            IsMatched = false;
            Columns = columns;
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
