using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace LiveBoard.Common
{
	public class NullToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (parameter == null || String.IsNullOrEmpty(parameter.ToString()))
				return value != null ? Visibility.Visible : Visibility.Collapsed;
			// '!' 등을 파라미터에 넣으면 반대로 해준다.
			return value != null ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}