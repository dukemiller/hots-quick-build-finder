using System;
using System.Globalization;
using System.Windows.Data;

namespace hots_quick_build_finder.Xaml
{
    public class PageEndCompareConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var start = System.Convert.ToInt32(values[0]) + 1;
            var end = System.Convert.ToInt32(values[1]);
            return start == end || start == 0 || end == 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
