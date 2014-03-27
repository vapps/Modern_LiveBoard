using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Data;

namespace LiveBoard.Common
{
	/// <summary>
	/// parameter에 쓰인 것을 'parameter: value' 식으로 나오게 함.
	/// </summary>
	public class PrefixConverter : IValueConverter
	{
		readonly ResourceLoader _loader = new ResourceLoader("Resources");

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (parameter == null)
				return value;

			var convertedParameter = _loader.GetString(parameter.ToString());
			if (String.IsNullOrEmpty(convertedParameter))
				return String.Format("{0}: {1}", parameter, value ?? _loader.GetString("NotAvailableAbbr/Text"));
			return String.Format("{0}: {1}", convertedParameter, value ?? _loader.GetString("NotAvailableAbbr/Text"));
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}