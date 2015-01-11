using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace Converters
{
    public class StringListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var stringList = value as List<string>;

            if (stringList != null && stringList.Count > 0)
            {
                return string.Join(", ", stringList.ToArray());
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
