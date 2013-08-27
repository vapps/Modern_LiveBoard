using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using LiveBoard.PageTemplate.Model;

namespace LiveBoard.Common
{
	public class PageExistToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			//return Visibility.Visible;
			if (!(value is int))
			{
				return Visibility.Collapsed;
			}
			return ((int) value)>0 ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}