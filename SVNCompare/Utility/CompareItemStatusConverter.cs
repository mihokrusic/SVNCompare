using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SVNCompare.Models;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;

namespace SVNCompare.Utility
{
    [ValueConversion(typeof(CompareItemStatus), typeof(Color))]
    public class CompareItemStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is CompareItemStatus))
                throw new ArgumentException("Value not of type CompareItemStatus");
            CompareItemStatus itemStatus = (CompareItemStatus)value;

            switch (itemStatus)
            {
                case CompareItemStatus.Base:
                    return new SolidColorBrush(Color.FromRgb(255, 235, 156));
                case CompareItemStatus.Identical:
                    return new SolidColorBrush(Color.FromRgb(198, 239, 206));
                case CompareItemStatus.Different:
                    return new SolidColorBrush(Color.FromRgb(255, 199, 206));
                case CompareItemStatus.Unknown:
                    return new SolidColorBrush(Colors.White);
                default:
                    return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new ArgumentException("Not implemented ConvertBack");
        }
    }
}
