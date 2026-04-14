using System.Globalization;
using System.Windows.Data;

namespace SciFi_IMDB.Services
{
    public class MillisecondsToTimeConverter : IValueConverter
    {
        // technically a minutes to timespan converter, i just left the name as is. the rename function was giving me trouble
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double minutes = 0;
            if (value is int mInt)
                minutes = mInt;
            else if
                (value is short mShort) minutes = mShort;
            else if
                (value is double mDouble) minutes = mDouble;
            else
                return "00:00";

            if (minutes <= 0) return "N/A";

            TimeSpan t = TimeSpan.FromMinutes(minutes);

            if (t.TotalHours >= 1)
            {
                // Format: H:MM:SS (e.g., 2:05:00)
                return string.Format("{0}:{1:D2}:{2:D2}", (int)t.TotalHours, t.Minutes, t.Seconds);
            }
            else
            {
                // Format: MM:SS (e.g., 45:00)
                return string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
