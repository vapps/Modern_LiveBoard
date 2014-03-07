using System;
using Windows.UI.Xaml.Data;

namespace LiveBoard.Common
{
	/// <summary>
	/// parameter에 쓰인 것을 'parameter: value' 식으로 나오게 함.
	/// </summary>
	public class ExtractFilenameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var path = value.ToString();
			if (String.IsNullOrEmpty(path))
				return path;
			path = System.IO.Path.GetFileNameWithoutExtension(Uri.UnescapeDataString(path).Replace("/", "\\"));
			return String.Format(path);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
	public static class StringExtensions
	{
		public static string Right(this string str, int length)
		{
			return str.Substring(str.Length - length, length);
		}
	}
}