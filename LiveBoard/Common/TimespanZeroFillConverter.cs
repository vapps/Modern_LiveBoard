using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Telerik.Charting;

namespace LiveBoard.Common
{
	public class TimespanZeroFillConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var duration = value.ToString();
			if (duration.Length < 2)
				return "0" + duration.ToString();
			return duration;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}