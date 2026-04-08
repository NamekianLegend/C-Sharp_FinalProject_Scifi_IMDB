using System.Globalization;
using System.Windows.Data;

namespace SciFi_IMDB.Services
{
    public class MillisecondsToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int ms)
            {
                TimeSpan t = TimeSpan.FromMilliseconds(ms);

                if (t.Hours > 0)
                {
                    // Format: H:MM:SS (e.g., 1:23:45)
                    return string.Format("{0}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
                }
                else
                {
                    // Format: MM:SS (e.g., 03:45)
                    return string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
                }
            }

            return "00:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
