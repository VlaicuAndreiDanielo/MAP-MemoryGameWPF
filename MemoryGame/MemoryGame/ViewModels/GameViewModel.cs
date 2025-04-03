using MemoryGame.Commands;
using MemoryGame.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            TimeRemaining = 60; // sau valoarea preluată din setup
            Cards = new ObservableCollection<CardViewModel>();
            GenerateCards(availableImages);
            CardFlipCommand = new RelayCommand(async (param) => await OnCardFlipped(param));
            PauseCommand = new RelayCommand(OpenPauseWindow);
            StartTimer();
        }

        // Constructor pentru continuarea unui joc salvat sau un joc nou
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
                var defaultImages = GetImages(game.Module);
                GenerateCards(defaultImages);
            }
            else
            {
                Cards = new ObservableCollection<CardViewModel>(
                    game.Cards.Select((cs, i) =>
                    {
                         var card = new CardViewModel(cs.ImagePath, Columns);
                         card.Index = i;
                        // Restaurează starea salvată:

                         card.IsMatched = cs.IsMatched;
                        // Dacă cardul este matched, păstrăm și starea flipuită; altfel, îl punem întotdeauna ca ne-flipuit.
                         card.IsFlipped = cs.IsMatched ? cs.IsFlipped : false;
                        // Notificăm UI-ul pentru actualizare:
                         card.OnPropertyChanged(nameof(card.IsFlipped));
                         card.OnPropertyChanged(nameof(card.IsMatched));
                         card.OnPropertyChanged(nameof(card.ShowFront));
                         return card;
                    })
                );

            }

            CardFlipCommand = new RelayCommand(async (param) => await OnCardFlipped(param));
            PauseCommand = new RelayCommand(OpenPauseWindow); // ADĂUGĂ LINIA ASTA**
            StartTimer();
        }


        public void PauseTimer()
        {
            // Oprește timerul dacă e pornit
            if (_timer != null)
                _timer.Stop();
        }

        public void ResumeTimer()
        {
            // Reia timerul dacă e oprit
            if (_timer != null)
                _timer.Start();
        }

        public Game GetCurrentGameState()
        {
            // Creează un obiect Game cu starea curentă
            // (modul, level, size, time, carduri)
            return new Game
            {
                // Dacă CurrentGame nu e null, preluăm Level și Module de acolo.
                // Sau, dacă preferi, poți folosi direct proprietăți separate.
                Level = CurrentGame != null ? CurrentGame.Level : "",
                Module = CurrentGame != null ? CurrentGame.Module : "",
                Rows = Rows,
                Columns = Columns,
                TimeRemaining = TimeRemaining,
                Cards = Cards.Select(c => new CardState
                {
                    ImagePath = c.FrontImage,
                    IsFlipped = c.IsFlipped,
                    IsMatched = c.IsMatched,     // <-- Adaugă această linie
                    Row = c.Row,
                    Column = c.Column
                }).ToList()

            };
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
                {
                    _timer.Stop();
                    // Deschide fereastra de Game Over pe thread-ul UI
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var gameOverView = new Views.GameOverView();
                        var gameOverVM = new GameOverViewModel(CurrentUser, CurrentGame ?? new Game { Rows = Rows, Columns = Columns, TimeRemaining = 0 });
                        gameOverView.DataContext = gameOverVM;
                        gameOverView.Show();
                        // Închide fereastra de joc
                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window is Views.GameView)
                            {
                                window.Close();
                                break;
                            }
                        }
                    });
                }
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
            if (Cards.All(c => c.IsMatched))
            {
                _timer.Stop();
                // Deschide fereastra de Win pe thread-ul UI
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var winView = new Views.WinView();
                    var winVM = new WinViewModel(CurrentUser, GetCurrentGameState());
                    winView.DataContext = winVM;
                    winView.Show();
                    // Închide fereastra de joc
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window is Views.GameView)
                        {
                            window.Close();
                            break;
                        }
                    }
                });
            }
        }
        // Metoda pentru deschiderea ferestrei de pauză
        private void OpenPauseWindow(object parameter)
        {
            PauseTimer();
            var pauseWindow = new Views.PauseWindow();
            pauseWindow.DataContext = new PauseViewModel(CurrentUser, this);
            pauseWindow.ShowDialog();
        }

        private static ObservableCollection<string> GetImages(string module)
        {
            switch (module)
            {
                case "Flowers":
                    return new ObservableCollection<string>
                {
                    "ImagesFlowers/flower1.png",  "ImagesFlowers/flower2.png",  "ImagesFlowers/flower3.png",
                    "ImagesFlowers/flower4.png",  "ImagesFlowers/flower5.png",  "ImagesFlowers/flower6.png",
                    "ImagesFlowers/flower7.png",  "ImagesFlowers/flower8.png",  "ImagesFlowers/flower9.png",
                    "ImagesFlowers/flower10.png", "ImagesFlowers/flower11.png", "ImagesFlowers/flower12.png",
                    "ImagesFlowers/flower13.png", "ImagesFlowers/flower14.png", "ImagesFlowers/flower15.png",
                    "ImagesFlowers/flower16.png", "ImagesFlowers/flower17.png", "ImagesFlowers/flower18.png",
                    "ImagesFlowers/flower19.png", "ImagesFlowers/flower20.png", "ImagesFlowers/flower21.png",
                    "ImagesFlowers/flower22.png", "ImagesFlowers/flower23.png", "ImagesFlowers/flower24.png",
                    "ImagesFlowers/flower25.png", "ImagesFlowers/flower26.png", "ImagesFlowers/flower27.png",
                    "ImagesFlowers/flower28.png", "ImagesFlowers/flower29.png", "ImagesFlowers/flower30.png",
                    "ImagesFlowers/flower31.png", "ImagesFlowers/flower32.png", "ImagesFlowers/flower33.png",
                    "ImagesFlowers/flower34.png", "ImagesFlowers/flower35.png", "ImagesFlowers/flower36.png",
                    "ImagesFlowers/flower37.png", "ImagesFlowers/flower38.png", "ImagesFlowers/flower39.png",
                    "ImagesFlowers/flower40.png", "ImagesFlowers/flower41.png", "ImagesFlowers/flower42.png",
                    "ImagesFlowers/flower43.png", "ImagesFlowers/flower44.png", "ImagesFlowers/flower45.png",
                    "ImagesFlowers/flower46.png", "ImagesFlowers/flower47.png", "ImagesFlowers/flower48.png",
                    "ImagesFlowers/flower49.png", "ImagesFlowers/flower50.png", "ImagesFlowers/flower51.png",
                    "ImagesFlowers/flower52.png", "ImagesFlowers/flower53.png", "ImagesFlowers/flower54.png",
                    "ImagesFlowers/flower55.png", "ImagesFlowers/flower56.png", "ImagesFlowers/flower57.png",
                    "ImagesFlowers/flower58.png", "ImagesFlowers/flower59.png", "ImagesFlowers/flower60.png",
                    "ImagesFlowers/flower61.png", "ImagesFlowers/flower62.png", "ImagesFlowers/flower63.png",
                    "ImagesFlowers/flower64.png", "ImagesFlowers/flower65.png", "ImagesFlowers/flower66.png",
                    "ImagesFlowers/flower67.png", "ImagesFlowers/flower68.png", "ImagesFlowers/flower69.png",
                    "ImagesFlowers/flower70.png", "ImagesFlowers/flower71.png"
                };

                case "Animals":
                    return new ObservableCollection<string>
                {
                    "ImagesAnimals/animal1.png", "ImagesAnimals/animal2.png", "ImagesAnimals/animal3.png",
                    "ImagesAnimals/animal4.png", "ImagesAnimals/animal5.png", "ImagesAnimals/animal6.png",
                    "ImagesAnimals/animal7.png", "ImagesAnimals/animal8.png", "ImagesAnimals/animal9.png",
                    "ImagesAnimals/animal10.png","ImagesAnimals/animal11.png","ImagesAnimals/animal12.png",
                    "ImagesAnimals/animal13.png","ImagesAnimals/animal14.png","ImagesAnimals/animal15.png",
                    "ImagesAnimals/animal16.png","ImagesAnimals/animal17.png","ImagesAnimals/animal18.png",
                    "ImagesAnimals/animal19.png","ImagesAnimals/animal20.png","ImagesAnimals/animal21.png",
                    "ImagesAnimals/animal22.png","ImagesAnimals/animal23.png","ImagesAnimals/animal24.png",
                    "ImagesAnimals/animal25.png","ImagesAnimals/animal26.png","ImagesAnimals/animal27.png",
                    "ImagesAnimals/animal28.png","ImagesAnimals/animal29.png","ImagesAnimals/animal30.png",
                    "ImagesAnimals/animal31.png","ImagesAnimals/animal32.png","ImagesAnimals/animal33.png",
                    "ImagesAnimals/animal34.png","ImagesAnimals/animal35.png","ImagesAnimals/animal36.png",
                    "ImagesAnimals/animal37.png","ImagesAnimals/animal38.png","ImagesAnimals/animal39.png",
                    "ImagesAnimals/animal40.png","ImagesAnimals/animal41.png","ImagesAnimals/animal42.png",
                    "ImagesAnimals/animal43.png","ImagesAnimals/animal44.png","ImagesAnimals/animal45.png",
                    "ImagesAnimals/animal46.png","ImagesAnimals/animal47.png","ImagesAnimals/animal48.png",
                    "ImagesAnimals/animal49.png","ImagesAnimals/animal50.png","ImagesAnimals/animal51.png",
                    "ImagesAnimals/animal52.png","ImagesAnimals/animal53.png","ImagesAnimals/animal54.png",
                    "ImagesAnimals/animal55.png","ImagesAnimals/animal56.png","ImagesAnimals/animal57.png",
                    "ImagesAnimals/animal58.png","ImagesAnimals/animal59.png","ImagesAnimals/animal60.png",
                    "ImagesAnimals/animal61.png","ImagesAnimals/animal62.png","ImagesAnimals/animal63.png",
                    "ImagesAnimals/animal64.png","ImagesAnimals/animal65.png","ImagesAnimals/animal66.png",
                    "ImagesAnimals/animal67.png","ImagesAnimals/animal68.png","ImagesAnimals/animal69.png",
                    "ImagesAnimals/animal70.png","ImagesAnimals/animal71.png","ImagesAnimals/animal72.png"
                };

                case "Trees":
                    return new ObservableCollection<string>
                {
                    "ImagesTrees/tree1.jpg", "ImagesTrees/tree2.jpg", "ImagesTrees/tree3.jpg",
                    "ImagesTrees/tree4.jpg", "ImagesTrees/tree5.jpg", "ImagesTrees/tree6.jpg",
                    "ImagesTrees/tree7.jpg", "ImagesTrees/tree8.jpg", "ImagesTrees/tree9.jpg",
                    "ImagesTrees/tree10.jpg","ImagesTrees/tree11.jpg","ImagesTrees/tree12.jpg",
                    "ImagesTrees/tree13.jpg","ImagesTrees/tree14.jpg","ImagesTrees/tree15.jpg",
                    "ImagesTrees/tree16.jpg","ImagesTrees/tree17.jpg","ImagesTrees/tree18.jpg",
                    "ImagesTrees/tree19.jpg","ImagesTrees/tree20.jpg","ImagesTrees/tree21.jpg",
                    "ImagesTrees/tree22.jpg","ImagesTrees/tree23.jpg","ImagesTrees/tree24.jpg",
                    "ImagesTrees/tree25.jpg","ImagesTrees/tree26.jpg","ImagesTrees/tree27.jpg",
                    "ImagesTrees/tree28.jpg","ImagesTrees/tree29.jpg","ImagesTrees/tree30.jpg",
                    "ImagesTrees/tree31.jpg","ImagesTrees/tree32.jpg","ImagesTrees/tree33.jpg",
                    "ImagesTrees/tree34.jpg","ImagesTrees/tree35.jpg","ImagesTrees/tree36.jpg",
                    "ImagesTrees/tree37.jpg","ImagesTrees/tree38.jpg","ImagesTrees/tree39.jpg",
                    "ImagesTrees/tree40.jpg","ImagesTrees/tree41.jpg","ImagesTrees/tree42.jpg"
                };

                case "Fruits":
                    return new ObservableCollection<string>
            {
                "ImagesFruits/fruit1.png", "ImagesFruits/fruit2.png"
                // etc.
            };
                case "Vegetables":
                    return new ObservableCollection<string>
                {
                    "ImagesVegetables/vegetable1.jpg","ImagesVegetables/vegetable2.jpg","ImagesVegetables/vegetable3.jpg",
                    "ImagesVegetables/vegetable4.jpg","ImagesVegetables/vegetable5.jpg","ImagesVegetables/vegetable6.jpg",
                    "ImagesVegetables/vegetable7.jpg","ImagesVegetables/vegetable8.jpg","ImagesVegetables/vegetable9.jpg",
                    "ImagesVegetables/vegetable10.jpg","ImagesVegetables/vegetable11.jpg","ImagesVegetables/vegetable12.jpg",
                    "ImagesVegetables/vegetable13.jpg","ImagesVegetables/vegetable14.jpg","ImagesVegetables/vegetable15.jpg",
                    "ImagesVegetables/vegetable16.jpg","ImagesVegetables/vegetable17.jpg","ImagesVegetables/vegetable18.jpg",
                    "ImagesVegetables/vegetable19.jpg","ImagesVegetables/vegetable20.jpg","ImagesVegetables/vegetable21.jpg",
                    "ImagesVegetables/vegetable22.jpg","ImagesVegetables/vegetable23.jpg","ImagesVegetables/vegetable24.jpg",
                    "ImagesVegetables/vegetable25.jpg","ImagesVegetables/vegetable26.jpg","ImagesVegetables/vegetable27.jpg",
                    "ImagesVegetables/vegetable28.jpg","ImagesVegetables/vegetable29.jpg","ImagesVegetables/vegetable30.jpg",
                    "ImagesVegetables/vegetable31.jpg","ImagesVegetables/vegetable32.jpg","ImagesVegetables/vegetable33.jpg",
                    "ImagesVegetables/vegetable34.jpg","ImagesVegetables/vegetable35.jpg","ImagesVegetables/vegetable36.jpg",
                    "ImagesVegetables/vegetable37.jpg","ImagesVegetables/vegetable38.jpg","ImagesVegetables/vegetable39.jpg",
                    "ImagesVegetables/vegetable40.jpg","ImagesVegetables/vegetable41.jpg","ImagesVegetables/vegetable42.jpg",
                    "ImagesVegetables/vegetable43.jpg","ImagesVegetables/vegetable44.jpg","ImagesVegetables/vegetable45.jpg",
                    "ImagesVegetables/vegetable46.jpg","ImagesVegetables/vegetable47.jpg","ImagesVegetables/vegetable48.jpg",
                    "ImagesVegetables/vegetable49.jpg","ImagesVegetables/vegetable50.jpg","ImagesVegetables/vegetable51.jpg",
                    "ImagesVegetables/vegetable52.jpg","ImagesVegetables/vegetable53.jpg","ImagesVegetables/vegetable54.jpg",
                    "ImagesVegetables/vegetable55.jpg","ImagesVegetables/vegetable56.jpg","ImagesVegetables/vegetable57.jpg",
                    "ImagesVegetables/vegetable58.jpg","ImagesVegetables/vegetable59.jpg","ImagesVegetables/vegetable60.jpg",
                    "ImagesVegetables/vegetable61.jpg","ImagesVegetables/vegetable62.jpg","ImagesVegetables/vegetable63.jpg"
                };

                case "Rocks":
                    return new ObservableCollection<string>
            {
                "ImagesRocks/rock1.png", "ImagesRocks/rock2.png"
                // etc.
            };
                case "Landscapes":
                    return new ObservableCollection<string>
            {
                "ImagesRocks/rock1.png", "ImagesRocks/rock2.png"
                // etc.
            };
                case "Buildings":
                    return new ObservableCollection<string>
            {
                "ImagesRocks/rock1.png", "ImagesRocks/rock2.png"
                // etc.
            };
                case "Motorcycle":
                    return new ObservableCollection<string>
            {
                "ImagesRocks/rock1.png", "ImagesRocks/rock2.png"
                // etc.
            };
                case "Cars":
                    return new ObservableCollection<string>
            {
                "ImagesRocks/rock1.png", "ImagesRocks/rock2.png"
                // etc.
            };
                case "Random":
                    return new ObservableCollection<string>
            {
                "ImagesRocks/rock1.png", "ImagesRocks/rock2.png"
                // etc.
            };
                default:
                    return new ObservableCollection<string>
                {
                    "ImagesFlowers/flower1.png",  "ImagesFlowers/flower2.png",  "ImagesFlowers/flower3.png",
                    "ImagesFlowers/flower4.png",  "ImagesFlowers/flower5.png",  "ImagesFlowers/flower6.png",
                    "ImagesFlowers/flower7.png",  "ImagesFlowers/flower8.png",  "ImagesFlowers/flower9.png",
                    "ImagesFlowers/flower10.png", "ImagesFlowers/flower11.png", "ImagesFlowers/flower12.png",
                    "ImagesFlowers/flower13.png", "ImagesFlowers/flower14.png", "ImagesFlowers/flower15.png",
                    "ImagesFlowers/flower16.png", "ImagesFlowers/flower17.png", "ImagesFlowers/flower18.png",
                    "ImagesFlowers/flower19.png", "ImagesFlowers/flower20.png", "ImagesFlowers/flower21.png",
                    "ImagesFlowers/flower22.png", "ImagesFlowers/flower23.png", "ImagesFlowers/flower24.png",
                    "ImagesFlowers/flower25.png", "ImagesFlowers/flower26.png", "ImagesFlowers/flower27.png",
                    "ImagesFlowers/flower28.png", "ImagesFlowers/flower29.png", "ImagesFlowers/flower30.png",
                    "ImagesFlowers/flower31.png", "ImagesFlowers/flower32.png", "ImagesFlowers/flower33.png",
                    "ImagesFlowers/flower34.png", "ImagesFlowers/flower35.png", "ImagesFlowers/flower36.png",
                    "ImagesFlowers/flower37.png", "ImagesFlowers/flower38.png", "ImagesFlowers/flower39.png",
                    "ImagesFlowers/flower40.png", "ImagesFlowers/flower41.png", "ImagesFlowers/flower42.png",
                    "ImagesFlowers/flower43.png", "ImagesFlowers/flower44.png", "ImagesFlowers/flower45.png",
                    "ImagesFlowers/flower46.png", "ImagesFlowers/flower47.png", "ImagesFlowers/flower48.png",
                    "ImagesFlowers/flower49.png", "ImagesFlowers/flower50.png", "ImagesFlowers/flower51.png",
                    "ImagesFlowers/flower52.png", "ImagesFlowers/flower53.png", "ImagesFlowers/flower54.png",
                    "ImagesFlowers/flower55.png", "ImagesFlowers/flower56.png", "ImagesFlowers/flower57.png",
                    "ImagesFlowers/flower58.png", "ImagesFlowers/flower59.png", "ImagesFlowers/flower60.png",
                    "ImagesFlowers/flower61.png", "ImagesFlowers/flower62.png", "ImagesFlowers/flower63.png",
                    "ImagesFlowers/flower64.png", "ImagesFlowers/flower65.png", "ImagesFlowers/flower66.png",
                    "ImagesFlowers/flower67.png", "ImagesFlowers/flower68.png", "ImagesFlowers/flower69.png",
                    "ImagesFlowers/flower70.png", "ImagesFlowers/flower71.png"
                };
            }
        }



        private void GenerateCards(ObservableCollection<string> availableImages)
        {
            int totalCards = Rows * Columns;
            int pairCount = totalCards / 2;
            var rnd = new Random();
            var selectedImages = new List<string>();

            // Dacă sunt destule imagini distincte, le selectează aleatoriu
            if (availableImages.Count >= pairCount)
            {
                selectedImages = availableImages.OrderBy(x => rnd.Next())
                                                .Take(pairCount)
                                                .ToList();
            }
            else
            {
                // Dacă nu sunt suficiente, alege aleatoriu (cu posibilitatea de repetare)
                for (int i = 0; i < pairCount; i++)
                {
                    int index = rnd.Next(availableImages.Count);
                    selectedImages.Add(availableImages[index]);
                }
            }

            // Dublează imaginile pentru a obține perechi și le amestecă
            var cardImages = selectedImages.Concat(selectedImages)
                                           .OrderBy(x => rnd.Next())
                                           .ToList();

            Cards = new ObservableCollection<CardViewModel>();
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
