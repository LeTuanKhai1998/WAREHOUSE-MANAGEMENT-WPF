using System;
using System.Globalization;
using System.Windows.Data;

namespace QuanLyKho.ViewModel
{
    public enum Status
    {
        All,
        Sale,
        Stop
    }

    public class StatusConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int p = 4;
            int.TryParse((string)parameter, out p);
            if (value is Status)
            {
                Status v = (Status)value;
                if (v == Status.All && p == 0)
                    return true;
                if (v == Status.Sale && p == 1)
                    return true;
                if (v == Status.Stop && p == 2)
                    return true;
            }
            return false;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int p = 4;
            int.TryParse((string)parameter, out p);
            if (value is bool && (bool)value)
                switch (p)
                {
                    case 0:
                        return Status.All;
                    case 1:
                        return Status.Sale;
                    case 2:
                        return Status.Stop;
                }
            return null;
        }
    }
}
