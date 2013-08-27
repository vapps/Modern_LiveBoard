using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LiveBoard.Common
{
	public class NumericMinuteConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value is TimeSpan)
				return ((TimeSpan) value).Minutes;
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}