using System;
using System.Windows.Data;
using xn;

namespace ScarySkeletons
{
    public class Point3DToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var point = (Point3D)value;
            return string.Format("{0}, {1}, {2}",
                point.X.ToString(), point.Y.ToString(), point.Z.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
