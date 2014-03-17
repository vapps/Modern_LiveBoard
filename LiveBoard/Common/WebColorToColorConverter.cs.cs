using System;
using System.Globalization;
using Windows.UI;
using Windows.UI.Xaml.Data;

namespace LiveBoard.Common
{
    /// <summary>
    /// Value converter that translates true to false and vice versa.
    /// </summary>
    public sealed class WebColorToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
	        if (value is String)
		        return convertToColor(value.ToString());
	        return Colors.Black; // default black.
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return !(value is bool && (bool)value);
        }

	    private static Color convertToColor(string colorCode)
	    {
			int argb = Int32.Parse(colorCode.Replace("#", ""), NumberStyles.HexNumber);
			return Color.FromArgb((byte)((argb & -16777216) >> 0x18),
								  (byte)((argb & 0xff0000) >> 0x10),
								  (byte)((argb & 0xff00) >> 8),
								  (byte)(argb & 0xff));
	    }
    }
}
