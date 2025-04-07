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
        public bool IsForcedQuit { get; set; } = true;

        private DispatcherTimer _timer;
        public ICommand CardFlipCommand { get; }
        public ICommand PauseCommand { get; set; }  

        private CardViewModel _firstFlippedCard = null;
        private bool _isChecking = false;
        public GameViewModel(User user, int rows, int columns, ObservableCollection<string> availableImages)
        {
            CurrentUser = user;
            Rows = rows;
            Columns = columns;
            TimeRemaining = 60;
            Cards = new ObservableCollection<CardViewModel>();
            GenerateCards(availableImages);
            CardFlipCommand = new RelayCommand(async (param) => await OnCardFlipped(param));
            PauseCommand = new RelayCommand(OpenPauseWindow);
            StartTimer();
        }

        public GameViewModel(User user, Game game)
        {
            CurrentUser = user;
            CurrentGame = game;
            Rows = game.Rows;
            Columns = game.Columns;
            TimeRemaining = game.TimeRemaining;
            if (game.Cards == null || game.Cards.Count == 0)
            {
                //CurrentGame.IsSaved = false; - Not Saved game
                Cards = new ObservableCollection<CardViewModel>();
                var defaultImages = GetImages(game.Module);
                GenerateCards(defaultImages);
            }
            else
            {
                //CurrentGame.IsSaved = true; - Saved game
                Cards = new ObservableCollection<CardViewModel>(
                    game.Cards.Select((cs, i) =>
                    {
                         var card = new CardViewModel(cs.ImagePath, Columns);
                         card.Index = i;

                         card.IsMatched = cs.IsMatched;
                         card.IsFlipped = cs.IsMatched ? cs.IsFlipped : false;

                         card.OnPropertyChanged(nameof(card.IsFlipped));
                         card.OnPropertyChanged(nameof(card.IsMatched));
                         card.OnPropertyChanged(nameof(card.ShowFront));
                         return card;
                    })
                );

            }

            CardFlipCommand = new RelayCommand(async (param) => await OnCardFlipped(param));
            PauseCommand = new RelayCommand(OpenPauseWindow); 
            StartTimer();
        }


        public void PauseTimer()
        {
            if (_timer != null)
                _timer.Stop();
        }

        public void ResumeTimer()
        {
            if (_timer != null)
                _timer.Start();
        }

        public Game GetCurrentGameState()
        {

            return new Game
            {
                Level = CurrentGame != null ? CurrentGame.Level : "",
                Module = CurrentGame != null ? CurrentGame.Module : "",
                Rows = Rows,
                Columns = Columns,
                TimeRemaining = TimeRemaining,
                Cards = Cards.Select(c => new CardState
                {
                    ImagePath = c.BackImage,
                    IsFlipped = c.IsFlipped,
                    IsMatched = c.IsMatched,
                    Row = c.Row,
                    Column = c.Column
                }).ToList(),
                IsSaved = CurrentGame?.IsSaved ?? false
            };
        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += (s, e) =>
            {
                if (Cards.All(c => c.IsMatched) && TimeRemaining > 0)
                {
                    _timer.Stop();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var winView = new Views.WinView();
                        var winVM = new WinViewModel(CurrentUser, GetCurrentGameState());
                        winView.DataContext = winVM;
                        winView.Show();
                        IsForcedQuit = false;
                        foreach (Window window in Application.Current.Windows.OfType<Window>().ToList())
                        {
                            if (window != winView)
                            {
                                window.Close();
                            }
                        }
                    });
                }
                if (TimeRemaining > 0)
                    TimeRemaining--;
                else
                {
                    _timer.Stop();

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var gameOverView = new Views.GameOverView();
                        var gameOverVM = new GameOverViewModel(CurrentUser, CurrentGame ?? new Game { Rows = Rows, Columns = Columns, TimeRemaining = 0 });
                        gameOverView.DataContext = gameOverVM;
                        gameOverView.Show();
                        IsForcedQuit = false;
                        foreach (Window window in Application.Current.Windows.OfType<Window>().ToList())
                        {
                            if (window != gameOverView)
                            {
                                window.Close();
                            }
                        }
                    });
                }
            };
            _timer.Start();
        }


        private async Task OnCardFlipped(object parameter)
        {
            if (_isChecking) return;
            if (parameter is CardViewModel card)
            {
                if (card.IsFlipped || card.IsMatched) return;

                card.Flip();

                if (_firstFlippedCard == null)
                {
                    _firstFlippedCard = card;
                }
                else if (_firstFlippedCard != card)
                {
                    if (_firstFlippedCard.BackImage == card.BackImage)
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
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var winView = new Views.WinView();
                    var winVM = new WinViewModel(CurrentUser, GetCurrentGameState());
                    winView.DataContext = winVM;
                    winView.Show();
                    IsForcedQuit = false;
                    foreach (Window window in Application.Current.Windows.OfType<Window>().ToList())
                    {
                        if (window != winView)
                        {
                            window.Close();
                        }
                    }
                });
            }
        }

        private void OpenPauseWindow(object parameter)
        {
            PauseTimer(); IsForcedQuit = false;
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
                    "ImagesFruits/fruit1.jpg",  "ImagesFruits/fruit2.jpg",  "ImagesFruits/fruit3.jpg",
                    "ImagesFruits/fruit4.jpg",  "ImagesFruits/fruit5.jpg",  "ImagesFruits/fruit6.jpg",
                    "ImagesFruits/fruit7.jpg",  "ImagesFruits/fruit8.jpg",  "ImagesFruits/fruit9.jpg",
                    "ImagesFruits/fruit10.jpg", "ImagesFruits/fruit11.jpg", "ImagesFruits/fruit12.jpg",
                    "ImagesFruits/fruit13.jpg", "ImagesFruits/fruit14.jpg", "ImagesFruits/fruit15.jpg",
                    "ImagesFruits/fruit16.jpg", "ImagesFruits/fruit17.jpg", "ImagesFruits/fruit18.jpg",
                    "ImagesFruits/fruit19.jpg", "ImagesFruits/fruit20.jpg", "ImagesFruits/fruit21.jpg",
                    "ImagesFruits/fruit22.jpg", "ImagesFruits/fruit23.jpg", "ImagesFruits/fruit24.jpg",
                    "ImagesFruits/fruit25.jpg", "ImagesFruits/fruit26.jpg", "ImagesFruits/fruit27.jpg",
                    "ImagesFruits/fruit28.jpg", "ImagesFruits/fruit29.jpg", "ImagesFruits/fruit30.jpg",
                    "ImagesFruits/fruit31.jpg", "ImagesFruits/fruit32.jpg", "ImagesFruits/fruit33.jpg",
                    "ImagesFruits/fruit34.jpg", "ImagesFruits/fruit35.jpg", "ImagesFruits/fruit36.jpg",
                    "ImagesFruits/fruit37.jpg", "ImagesFruits/fruit38.jpg", "ImagesFruits/fruit39.jpg",
                    "ImagesFruits/fruit40.jpg", "ImagesFruits/fruit41.jpg", "ImagesFruits/fruit42.jpg",
                    "ImagesFruits/fruit43.jpg", "ImagesFruits/fruit44.jpg", "ImagesFruits/fruit45.jpg",
                    "ImagesFruits/fruit46.jpg", "ImagesFruits/fruit47.jpg", "ImagesFruits/fruit48.jpg",
                    "ImagesFruits/fruit49.jpg", "ImagesFruits/fruit50.jpg", "ImagesFruits/fruit51.jpg",
                    "ImagesFruits/fruit52.jpg", "ImagesFruits/fruit53.jpg", "ImagesFruits/fruit54.jpg",
                    "ImagesFruits/fruit55.jpg", "ImagesFruits/fruit56.jpg", "ImagesFruits/fruit57.jpg",
                    "ImagesFruits/fruit58.jpg", "ImagesFruits/fruit59.jpg", "ImagesFruits/fruit60.jpg",
                    "ImagesFruits/fruit61.jpg", "ImagesFruits/fruit62.jpg", "ImagesFruits/fruit63.jpg",
                    "ImagesFruits/fruit64.jpg", "ImagesFruits/fruit65.jpg", "ImagesFruits/fruit66.jpg",
                    "ImagesFruits/fruit67.jpg", "ImagesFruits/fruit68.jpg", "ImagesFruits/fruit69.jpg",
                    "ImagesFruits/fruit70.jpg", "ImagesFruits/fruit71.jpg", "ImagesFruits/fruit72.jpg",
                    "ImagesFruits/fruit73.jpg", "ImagesFruits/fruit74.jpg", "ImagesFruits/fruit75.jpg",
                    "ImagesFruits/fruit76.jpg", "ImagesFruits/fruit77.jpg", "ImagesFruits/fruit78.jpg",
                    "ImagesFruits/fruit79.jpg", "ImagesFruits/fruit80.jpg", "ImagesFruits/fruit81.jpg",
                    "ImagesFruits/fruit82.jpg", "ImagesFruits/fruit83.jpg", "ImagesFruits/fruit84.jpg",
                    "ImagesFruits/fruit85.jpg", "ImagesFruits/fruit86.jpg", "ImagesFruits/fruit87.jpg",
                    "ImagesFruits/fruit88.jpg", "ImagesFruits/fruit89.jpg", "ImagesFruits/fruit90.jpg",
                    "ImagesFruits/fruit91.jpg", "ImagesFruits/fruit92.jpg", "ImagesFruits/fruit93.jpg",
                    "ImagesFruits/fruit94.jpg", "ImagesFruits/fruit95.jpg"
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
                    "ImagesRocks/rock1.jpg",  "ImagesRocks/rock2.jpg",  "ImagesRocks/rock3.jpg",
                    "ImagesRocks/rock4.jpg",  "ImagesRocks/rock5.jpg",  "ImagesRocks/rock6.jpg",
                    "ImagesRocks/rock7.jpg",  "ImagesRocks/rock8.jpg",  "ImagesRocks/rock9.jpg",
                    "ImagesRocks/rock10.jpg", "ImagesRocks/rock11.jpg", "ImagesRocks/rock12.jpg",
                    "ImagesRocks/rock13.jpg", "ImagesRocks/rock14.jpg", "ImagesRocks/rock15.jpg",
                    "ImagesRocks/rock16.jpg", "ImagesRocks/rock17.jpg", "ImagesRocks/rock18.jpg",
                    "ImagesRocks/rock19.jpg", "ImagesRocks/rock20.jpg", "ImagesRocks/rock21.jpg",
                    "ImagesRocks/rock22.jpg", "ImagesRocks/rock23.jpg", "ImagesRocks/rock24.jpg",
                    "ImagesRocks/rock25.jpg", "ImagesRocks/rock26.jpg", "ImagesRocks/rock27.jpg",
                    "ImagesRocks/rock28.jpg", "ImagesRocks/rock29.jpg", "ImagesRocks/rock30.jpg",
                    "ImagesRocks/rock31.jpg", "ImagesRocks/rock32.jpg", "ImagesRocks/rock33.jpg",
                    "ImagesRocks/rock34.jpg", "ImagesRocks/rock35.jpg", "ImagesRocks/rock36.jpg",
                    "ImagesRocks/rock37.jpg", "ImagesRocks/rock38.jpg", "ImagesRocks/rock39.jpg",
                    "ImagesRocks/rock40.jpg", "ImagesRocks/rock41.jpg", "ImagesRocks/rock42.jpg",
                    "ImagesRocks/rock43.jpg", "ImagesRocks/rock44.jpg", "ImagesRocks/rock45.jpg",
                    "ImagesRocks/rock46.jpg", "ImagesRocks/rock47.jpg", "ImagesRocks/rock48.jpg",
                    "ImagesRocks/rock49.jpg", "ImagesRocks/rock50.jpg", "ImagesRocks/rock51.jpg",
                    "ImagesRocks/rock52.jpg", "ImagesRocks/rock53.jpg", "ImagesRocks/rock54.jpg",
                    "ImagesRocks/rock55.jpg", "ImagesRocks/rock56.jpg", "ImagesRocks/rock57.jpg",
                    "ImagesRocks/rock58.jpg", "ImagesRocks/rock59.jpg", "ImagesRocks/rock60.jpg",
                    "ImagesRocks/rock61.jpg", "ImagesRocks/rock62.jpg", "ImagesRocks/rock63.jpg",
                    "ImagesRocks/rock64.jpg", "ImagesRocks/rock65.jpg", "ImagesRocks/rock66.jpg",
                    "ImagesRocks/rock67.jpg", "ImagesRocks/rock68.jpg", "ImagesRocks/rock69.jpg",
                    "ImagesRocks/rock70.jpg", "ImagesRocks/rock71.jpg", "ImagesRocks/rock72.jpg",
                    "ImagesRocks/rock73.jpg", "ImagesRocks/rock74.jpg", "ImagesRocks/rock75.jpg",
                    "ImagesRocks/rock76.jpg", "ImagesRocks/rock77.jpg", "ImagesRocks/rock78.jpg",
                    "ImagesRocks/rock79.jpg", "ImagesRocks/rock80.jpg", "ImagesRocks/rock81.jpg",
                    "ImagesRocks/rock82.jpg", "ImagesRocks/rock83.jpg", "ImagesRocks/rock84.jpg",
                    "ImagesRocks/rock85.jpg", "ImagesRocks/rock86.jpg", "ImagesRocks/rock87.jpg",
                    "ImagesRocks/rock88.jpg", "ImagesRocks/rock89.jpg", "ImagesRocks/rock90.jpg",
                    "ImagesRocks/rock91.jpg", "ImagesRocks/rock92.jpg", "ImagesRocks/rock93.jpg",
                    "ImagesRocks/rock94.jpg", "ImagesRocks/rock95.jpg", "ImagesRocks/rock96.jpg",
                    "ImagesRocks/rock97.jpg", "ImagesRocks/rock98.jpg", "ImagesRocks/rock99.jpg",
                    "ImagesRocks/rock100.jpg","ImagesRocks/rock101.jpg","ImagesRocks/rock102.jpg"
                };

                case "Landscapes":
                    return new ObservableCollection<string>
                {
                    "ImagesLandscapes/landscape1.jpg",  "ImagesLandscapes/landscape2.jpg",  "ImagesLandscapes/landscape3.jpg",
                    "ImagesLandscapes/landscape4.jpg",  "ImagesLandscapes/landscape5.jpg",  "ImagesLandscapes/landscape6.jpg",
                    "ImagesLandscapes/landscape7.jpg",  "ImagesLandscapes/landscape8.jpg",  "ImagesLandscapes/landscape9.jpg",
                    "ImagesLandscapes/landscape10.jpg", "ImagesLandscapes/landscape11.jpg", "ImagesLandscapes/landscape12.jpg",
                    "ImagesLandscapes/landscape13.jpg", "ImagesLandscapes/landscape14.jpg", "ImagesLandscapes/landscape15.jpg",
                    "ImagesLandscapes/landscape16.jpg", "ImagesLandscapes/landscape17.jpg", "ImagesLandscapes/landscape18.jpg",
                    "ImagesLandscapes/landscape19.jpg", "ImagesLandscapes/landscape20.jpg", "ImagesLandscapes/landscape21.jpg",
                    "ImagesLandscapes/landscape22.jpg", "ImagesLandscapes/landscape23.jpg", "ImagesLandscapes/landscape24.jpg",
                    "ImagesLandscapes/landscape25.jpg", "ImagesLandscapes/landscape26.jpg", "ImagesLandscapes/landscape27.jpg",
                    "ImagesLandscapes/landscape28.jpg", "ImagesLandscapes/landscape29.jpg", "ImagesLandscapes/landscape30.jpg",
                    "ImagesLandscapes/landscape31.jpg", "ImagesLandscapes/landscape32.jpg", "ImagesLandscapes/landscape33.jpg",
                    "ImagesLandscapes/landscape34.jpg", "ImagesLandscapes/landscape35.jpg", "ImagesLandscapes/landscape36.jpg",
                    "ImagesLandscapes/landscape37.jpg", "ImagesLandscapes/landscape38.jpg", "ImagesLandscapes/landscape39.jpg",
                    "ImagesLandscapes/landscape40.jpg", "ImagesLandscapes/landscape41.jpg", "ImagesLandscapes/landscape42.jpg",
                    "ImagesLandscapes/landscape43.jpg", "ImagesLandscapes/landscape44.jpg", "ImagesLandscapes/landscape45.jpg",
                    "ImagesLandscapes/landscape46.jpg", "ImagesLandscapes/landscape47.jpg", "ImagesLandscapes/landscape48.jpg",
                    "ImagesLandscapes/landscape49.jpg", "ImagesLandscapes/landscape50.jpg", "ImagesLandscapes/landscape51.jpg",
                    "ImagesLandscapes/landscape52.jpg", "ImagesLandscapes/landscape53.jpg", "ImagesLandscapes/landscape54.jpg",
                    "ImagesLandscapes/landscape55.jpg", "ImagesLandscapes/landscape56.jpg", "ImagesLandscapes/landscape57.jpg",
                    "ImagesLandscapes/landscape58.jpg", "ImagesLandscapes/landscape59.jpg", "ImagesLandscapes/landscape60.jpg",
                    "ImagesLandscapes/landscape61.jpg", "ImagesLandscapes/landscape62.jpg", "ImagesLandscapes/landscape63.jpg",
                    "ImagesLandscapes/landscape64.jpg", "ImagesLandscapes/landscape65.jpg", "ImagesLandscapes/landscape66.jpg"
                };

                case "Buildings":
                    MessageBox.Show("This category does not have the images yet! Come back tomorrow=))");
                    return new ObservableCollection<string>{"ImagesSorry/sorry1.png", "ImagesSorry/sorry2.png", "ImagesSorry/sorry3.png", "ImagesSorry/sorry4.png" };
                case "Motorcycle":
                    MessageBox.Show("This category does not have the images yet! Come back tomorrow=))");
                    return new ObservableCollection<string> { "ImagesSorry/sorry1.png", "ImagesSorry/sorry2.png", "ImagesSorry/sorry3.png", "ImagesSorry/sorry4.png" };
                case "Cars":
                    MessageBox.Show("This category does not have the images yet! Come back tomorrow=))");
                    return new ObservableCollection<string> { "ImagesSorry/sorry1.png", "ImagesSorry/sorry2.png", "ImagesSorry/sorry3.png", "ImagesSorry/sorry4.png" };
                case "Tool":
                    MessageBox.Show("This category does not have the images yet! Come back tomorrow=))");
                    return new ObservableCollection<string> { "ImagesSorry/sorry1.png", "ImagesSorry/sorry2.png", "ImagesSorry/sorry3.png", "ImagesSorry/sorry4.png" };
                case "Random":
                    MessageBox.Show("This category does not have the images yet! Come back tomorrow=))");
                   return new ObservableCollection<string>{"ImagesSorry/sorry1.png", "ImagesSorry/sorry2.png", "ImagesSorry/sorry3.png", "ImagesSorry/sorry4.png" };
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

            if (availableImages.Count >= pairCount)
            {
                selectedImages = availableImages.OrderBy(x => rnd.Next())
                                                .Take(pairCount)
                                                .ToList();
            }
            else
            {
                for (int i = 0; i < pairCount; i++)
                {
                    int index = rnd.Next(availableImages.Count);
                    selectedImages.Add(availableImages[index]);
                }
            }

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
