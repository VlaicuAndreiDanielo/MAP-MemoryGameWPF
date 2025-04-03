
using MemoryGame.Commands;
using MemoryGame.Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace MemoryGame.ViewModels
{
    public class ThemePickerViewModel
    {
        public ObservableCollection<string> AvailableThemes { get; }
        public string SelectedTheme { get; set; }

        public ICommand ApplyThemeCommand { get; }

        public ThemePickerViewModel()
        {
            AvailableThemes = new ObservableCollection<string>(ThemeManager.AvailableThemes);
            SelectedTheme = Settings.Default.SelectedTheme; // or read from user settings
            ApplyThemeCommand = new RelayCommand(ApplyTheme);
        }

        private void ApplyTheme(object parameter)
        {
            if (!string.IsNullOrEmpty(SelectedTheme))
            {
                ThemeManager.ApplyTheme(SelectedTheme);
                Settings.Default.SelectedTheme = SelectedTheme;
                Settings.Default.Save();    
            }
            // Close the window if you want:
            if (parameter is Window w) w.Close();
        }
    }
}
