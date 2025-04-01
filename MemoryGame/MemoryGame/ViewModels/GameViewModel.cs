using MemoryGame.Commands;
using MemoryGame.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace MemoryGame.ViewModels
{
    public class GameViewModel : INotifyPropertyChanged
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public ObservableCollection<CardViewModel> Cards { get; set; }
        public User CurrentUser { get; set; }
        public Game CurrentGame { get; set; }

        private int _timeRemaining;
        public int TimeRemaining
        {
            get => _timeRemaining;
            set { _timeRemaining = value; OnPropertyChanged(nameof(TimeRemaining)); }
        }

        // Constructor pentru continuarea unui joc salvat
        private DispatcherTimer _timer;
        public ICommand CardFlipCommand { get; }
        public ICommand PauseCommand { get; set; }  // Poți defini logica de pauză

        // Variabilă pentru a reține primul card flip-uit
        private CardViewModel _firstFlippedCard = null;
        private bool _isChecking = false;
        // Constructor pentru joc nou cu lista de imagini
        public GameViewModel(User user, int rows, int columns, ObservableCollection<string> availableImages)
        {
            CurrentUser = user;
            Rows = rows;
            Columns = columns;
            TimeRemaining = 60; // Sau setează valoarea preluată din GameSetup
            Cards = new ObservableCollection<CardViewModel>();
            GenerateCards(availableImages);
            CardFlipCommand = new RelayCommand(async (param) => await OnCardFlipped(param));
            StartTimer();
        }

     

        // Constructor pentru continuarea unui joc salvat sau un joc nou
        public GameViewModel(User user, Game game)
        {
            CurrentUser = user;
            CurrentGame = game;
            Rows = game.Rows;
            Columns = game.Columns;
            TimeRemaining = game.TimeRemaining; // preia timpul din setup

            if (game.Cards == null || game.Cards.Count == 0)
            {
                Cards = new ObservableCollection<CardViewModel>();
                GenerateCards(GetDefaultImages());
            }
            else
            {
                Cards = new ObservableCollection<CardViewModel>(
                    game.Cards.Select((cs, i) => {
                        var card = new CardViewModel(cs.ImagePath, Columns);
                        card.Index = i;
                        return card;
                    })
                );
            }
            CardFlipCommand = new RelayCommand(async (param) => await OnCardFlipped(param));
            StartTimer();
        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) =>
            {
                if (TimeRemaining > 0)
                    TimeRemaining--;
                else
                    _timer.Stop();
            };
            _timer.Start();
        }

        // Metoda OnCardFlipped rămâne la fel, doar scoți logica cu CurrentImage
        private async Task OnCardFlipped(object parameter)
        {
            if (_isChecking) return;
            if (parameter is CardViewModel card)
            {
                if (card.IsFlipped || card.IsMatched) return;

                card.Flip(); // Doar togglăm IsFlipped

                if (_firstFlippedCard == null)
                {
                    _firstFlippedCard = card;
                }
                else if (_firstFlippedCard != card)
                {
                    if (_firstFlippedCard.FrontImage == card.FrontImage)
                    {
                        _firstFlippedCard.SetMatched(true);
                        card.SetMatched(true);
                        _firstFlippedCard = null;
                    }
                    else
                    {
                        _isChecking = true;
                        await Task.Delay(1000);
                        _firstFlippedCard.Flip();
                        card.Flip();
                        _firstFlippedCard = null;
                        _isChecking = false;
                    }
                }
            }
        }

        private static ObservableCollection<string> GetDefaultImages()
        {
            return new ObservableCollection<string>
    {
        "ImagesFlowers/flower1.png", "ImagesFlowers/flower2.png", "ImagesFlowers/flower3.png", "ImagesFlowers/flower4.png", "ImagesFlowers/flower5.png", "ImagesFlowers/flower6.png",
        "ImagesFlowers/flower7.png", "ImagesFlowers/flower8.png", "ImagesFlowers/flower9.png", "ImagesFlowers/flower10.png","ImagesFlowers/flower11.png","ImagesFlowers/flower12.png",
        "ImagesFlowers/flower13.png","ImagesFlowers/flower14.png","ImagesFlowers/flower15.png","ImagesFlowers/flower16.png","ImagesFlowers/flower17.png","ImagesFlowers/flower18.png",
        "ImagesFlowers/flower19.png","ImagesFlowers/flower20.png","ImagesFlowers/flower21.png","ImagesFlowers/flower22.png","ImagesFlowers/flower23.png","ImagesFlowers/flower24.png",
        "ImagesFlowers/flower25.png","ImagesFlowers/flower26.png","ImagesFlowers/flower27.png","ImagesFlowers/flower28.png","ImagesFlowers/flower29.png","ImagesFlowers/flower30.png",
        "ImagesFlowers/flower31.png","ImagesFlowers/flower32.png","ImagesFlowers/flower33.png","ImagesFlowers/flower34.png","ImagesFlowers/flower35.png","ImagesFlowers/flower36.png",
        "ImagesFlowers/flower37.png","ImagesFlowers/flower38.png","ImagesFlowers/flower39.png","ImagesFlowers/flower40.png","ImagesFlowers/flower41.png","ImagesFlowers/flower42.png",
        "ImagesFlowers/flower43.png","ImagesFlowers/flower44.png","ImagesFlowers/flower45.png","ImagesFlowers/flower46.png","ImagesFlowers/flower47.png","ImagesFlowers/flower48.png",
        "ImagesFlowers/flower49.png","ImagesFlowers/flower50.png","ImagesFlowers/flower51.png","ImagesFlowers/flower52.png","ImagesFlowers/flower53.png","ImagesFlowers/flower54.png",
        "ImagesFlowers/flower55.png","ImagesFlowers/flower56.png","ImagesFlowers/flower57.png","ImagesFlowers/flower58.png","ImagesFlowers/flower59.png","ImagesFlowers/flower60.png",
        "ImagesFlowers/flower61.png","ImagesFlowers/flower62.png","ImagesFlowers/flower63.png","ImagesFlowers/flower64.png","ImagesFlowers/flower65.png","ImagesFlowers/flower66.png",
        "ImagesFlowers/flower67.png","ImagesFlowers/flower68.png","ImagesFlowers/flower69.png","ImagesFlowers/flower70.png","ImagesFlowers/flower71.png"
    };
        }


        private void GenerateCards(ObservableCollection<string> availableImages)
        {
            int totalCards = Rows * Columns;
            int pairCount = totalCards / 2;
            var rnd = new Random();

            // Dacă nu avem destule imagini, repetă-le
            var selectedImages = new List<string>();
            for (int i = 0; i < pairCount; i++)
            {
                selectedImages.Add(availableImages[i % availableImages.Count]);
            }

            // Amestecă și dublează pentru a obține perechi
            selectedImages = selectedImages.OrderBy(x => rnd.Next()).ToList();
            var cardImages = selectedImages.Concat(selectedImages).OrderBy(x => rnd.Next()).ToList();

            // Generează cardurile și setează indexul fiecăruia
            for (int i = 0; i < cardImages.Count; i++)
            {
                var card = new CardViewModel(cardImages[i], Columns);
                card.Index = i;
                Cards.Add(card);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
