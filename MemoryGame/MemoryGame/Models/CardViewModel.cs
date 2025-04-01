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

        // Proprietate care returnează true dacă cardul e flip-uit sau e matched
        public bool ShowFront => IsFlipped || IsMatched;

        public CardViewModel(string frontImage, int columns)
        {
            FrontImage = "../" + frontImage;
            // Imaginea inițială (față de spate)
            // Folosim pack URI pentru a fi sigur că WPF găsește resursa
            // Asumăm că folderul se numește ImagesCards și se află la rădăcina proiectului
            // (sau ajustează calea conform structurii tale)
            IsFlipped = false;
            IsMatched = false;
            Columns = columns;
        }

        public void Flip()
        {
            if (IsMatched)
                return; // dacă a fost deja găsit, nu mai se flip
            IsFlipped = !IsFlipped;
            OnPropertyChanged(nameof(IsFlipped));
            OnPropertyChanged(nameof(ShowFront));
        }

        // Dacă setezi direct IsMatched, asigură-te că notifici și ShowFront
        public void SetMatched(bool matched)
        {
            IsMatched = matched;
            OnPropertyChanged(nameof(IsMatched));
            OnPropertyChanged(nameof(ShowFront));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
          => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
