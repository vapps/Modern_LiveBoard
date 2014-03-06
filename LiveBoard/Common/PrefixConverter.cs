using System;
using Windows.UI.Xaml.Data;

namespace LiveBoard.Common
{
	/// <summary>
	/// parameter에 쓰인 것을 'parameter: value' 식으로 나오게 함.
	/// </summary>
	public class PrefixConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (parameter == null)
				return value;
			return String.Format("{0}: {1}", parameter, value ?? "N/A");
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}