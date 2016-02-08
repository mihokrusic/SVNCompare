using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Globalization;
using SVNModels;

namespace SVNCompare.Utility
{
    [ValueConversion(typeof(CompareFileStatus), typeof(Color))]
    class CompareFileStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is CompareFileStatus))
                throw new ArgumentException("Value not of type CompareItemStatus");
            CompareFileStatus itemStatus = (CompareFileStatus)value;

            switch (itemStatus)
            {
                case CompareFileStatus.Identical:
                    return new SolidColorBrush(Color.FromRgb(198, 239, 206));
                case CompareFileStatus.Different:
                    return new SolidColorBrush(Color.FromRgb(255, 199, 206));
                case CompareFileStatus.Unique:
                case CompareFileStatus.BaseUnique:
                    return new SolidColorBrush(Color.FromRgb(163, 221, 255));
                case CompareFileStatus.Unknown:
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