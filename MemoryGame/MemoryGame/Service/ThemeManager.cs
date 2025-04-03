using System;
using System.Collections.Generic;
using System.Windows;

namespace MemoryGame.Services
{
    public static class ThemeManager
    {
        public static List<string> AvailableThemes { get; } = new List<string>
        {
            "HeavenLight", "TotalDarkness", "ForestGreen", "QuantumRed", "DeepBlue", 
            "DeepOrange", "IntenseViolet", "PaleLavender", "SunnyYellow", "BabyPink",
        };

        public static void ApplyTheme(string themeName)
        {
            for (int i = 0; i < Application.Current.Resources.MergedDictionaries.Count; i++)
            {
                var dict = Application.Current.Resources.MergedDictionaries[i];
                if (dict.Source != null && dict.Source.OriginalString.Contains("Theme"))
                {
                    Application.Current.Resources.MergedDictionaries.RemoveAt(i);
                    i--;
                }
            }

            ResourceDictionary newDict = new ResourceDictionary();
            try
            {
                newDict.Source = new Uri($"/MemoryGame;component/Themes/{themeName}Theme.xaml", UriKind.Relative);
                Application.Current.Resources.MergedDictionaries.Add(newDict);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Eroare la aplicarea temei: " + ex.Message);
            }
        }

        public static void SetDefaultTheme()
        {
            ApplyTheme("TotalDarkness");
        }
    }
}
