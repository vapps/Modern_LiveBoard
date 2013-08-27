using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LiveBoard.Common
{
	public class PageToCountDisplayConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (!(value is int))
				return null;
			var index = ((int) value);
			if (index < 0)
				return 0;
			return index + 1;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}