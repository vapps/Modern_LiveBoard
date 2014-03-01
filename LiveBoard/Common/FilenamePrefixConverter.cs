using System;
using Windows.UI.Xaml.Data;

namespace LiveBoard.Common
{
	public class FilenamePrefixConverter : IValueConverter
	{
		private const string prefix = "File";
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (value == null)
				return String.Format("{0}: {1}", prefix, "Untitled.lbd");
			return String.Format("{0}: {1}", prefix, value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}