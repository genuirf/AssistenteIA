using System;
using System.Windows.Data;
using static Gf.AssistenteIA.Utils.Funcoes;

namespace Gf.AssistenteIA.Converters
{
      public class BooleanInversoConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !CBool(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return !CBool(value);
        }
    }
}
