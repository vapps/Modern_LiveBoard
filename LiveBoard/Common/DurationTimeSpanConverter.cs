using System;
using Windows.UI.Xaml.Data;

namespace LiveBoard.Common
{
	public class DurationTimeSpanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (String.IsNullOrEmpty(parameter.ToString()))
				throw new ArgumentNullException("parameter", "parameter should be either 'minute' or 'second'");
			if (parameter.ToString().ToLower().Equals("minute"))
			{
				if (value is TimeSpan)
				{
					var minute = ((TimeSpan)value).Hours * 60 + ((TimeSpan)value).Minutes;
					return minute;
				}
				return 0;
			}
			if (parameter.ToString().ToLower().Equals("second"))
			{
				if (value is TimeSpan)
					return ((TimeSpan)value).Seconds;
				return 0;
			}
			return 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}