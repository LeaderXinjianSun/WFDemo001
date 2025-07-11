using System;
using System.Globalization;
using System.Windows.Data;

namespace WFDemo001.Design.Converts
{
    public sealed class BooleanReversalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool v1)
            {
                return !v1;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return default(bool);
        }
    }
}
