
using Microsoft.VisualBasic;
using System.Data;
using System.Diagnostics;
using System.Windows.Data;

namespace Gf.AssistenteIA.Converters
{
      public class NumericValueConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Information.IsNumeric(value) && !double.IsNaN(System.Convert.ToDouble(value)) && !float.IsNaN(System.Convert.ToSingle(value)))
            {
                return value;
            }
            else
            {
                return parameter;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object ret = parameter;
            if (Information.IsNumeric(value))
            {
                if (targetType != null)
                {
                    try
                    {
                        ret = Conversion.CTypeDynamic(value, targetType);
                    }
                    catch
                    {
                        ret = parameter;
                    }
                }
            }
            else if (value != null && !Equals(value, DBNull.Value) && value.GetType() == typeof(string))
            {
                try
                {
                    if (IsExpression(System.Convert.ToString(value)))
                    {
                        DataTable tb = new DataTable();
                        ret = tb.Compute(System.Convert.ToString(value), null);
                    }
                }
                catch
                {
                    ret = parameter;
                }
            }
            return ret;
        }

        [DebuggerHidden()]
        private bool IsExpression(string v)
        {
            if (v.Contains("+") || v.Contains("-") || v.Contains("*") || v.Contains("/") || v.Contains("(") || v.Contains(")"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
