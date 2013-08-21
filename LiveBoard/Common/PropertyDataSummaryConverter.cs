using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace LiveBoard.Common
{
	/// <summary>
	/// Value converter that translates true to <see cref="Visibility.Visible"/> and false to
	/// <see cref="Visibility.Collapsed"/>.
	/// </summary>
	public sealed class PropertyDataSummaryConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var visible = (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
			if (parameter == null)
				return visible;
			// input whatever thing (such as '!') in XAML converter parameter will change its visiblity output.
			if (parameter.ToString().Length > 0)
				return visible == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
			return visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			var result = value is Visibility && (Visibility)value == Visibility.Visible;
			if (parameter == null)
				return result;

			if (parameter.ToString().Length > 0)
				return !result;
			return result;
		}
	}
}
