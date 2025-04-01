using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace MemoryGame.Converters
{
    public class StringToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    var image = new BitmapImage();
                    image.BeginInit();
                    // Se specifică UriKind.RelativeOrAbsolute pentru a suporta căi relative
                    image.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.EndInit();
                    return image;
                }
                catch (Exception ex)
                {
                    // Poți loga excepția dacă este necesar
                    return null;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
