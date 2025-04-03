using MemoryGame.Services;
using MemoryGame.Views;
using System.Configuration;
using System.Data;
using System.Windows;

namespace MemoryGame
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            string memoryGameTheme = Settings.Default.SelectedTheme;
            switch (memoryGameTheme)
            {
                case "HeavenLight":
                    ThemeManager.ApplyTheme("HeavenLight");
                    break;
                case "TotalDarkness":
                    ThemeManager.ApplyTheme("TotalDarkness");
                    break;
                case "ForestGreen":
                    ThemeManager.ApplyTheme("ForestGreen");
                    break;
                case "QuantumRed":
                    ThemeManager.ApplyTheme("QuantumRed");
                    break;
                case "DeepBlue":
                    ThemeManager.ApplyTheme("DeepBlue");
                    break;
                case "PaleLavender":
                    ThemeManager.ApplyTheme("PaleLavender");
                    break;
                case "SunnyYellow":
                    ThemeManager.ApplyTheme("SunnyYellow");
                    break;
                case "DeepOrange":
                    ThemeManager.ApplyTheme("DeepOrange");
                    break;
                case "IntenseViolet":
                    ThemeManager.ApplyTheme("IntenseViolet");
                    break;
                case "BabyPink":
                    ThemeManager.ApplyTheme("BabyPink");
                    break;
                default:
                    ThemeManager.SetDefaultTheme();
                    break;
            }

        }

    }

}
