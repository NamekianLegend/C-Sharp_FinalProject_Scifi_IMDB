using System;
using System.Globalization;
using System.Windows.Data;

namespace A2_Chinook_EFandLINQ.Services
{
    public class MillisecondsToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int ms)
            {
                TimeSpan t = TimeSpan.FromMilliseconds(ms);
                // Formats as Minutes:Seconds (e.g., 3:45)
                return string.Format("{0}:{1:D2}", t.Minutes + (t.Hours * 60), t.Seconds);
            }
            return "00:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}