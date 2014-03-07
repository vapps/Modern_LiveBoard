using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace LiveBoard.Common
{
	public class TemplateViewToIconImage : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			const string prefix = "ms-appx:///Images/";
			var uri = new Uri(prefix + "appbar.warning.png");
			try
			{

				switch (value.ToString())
				{
					case "SingleStringPage":
						uri = new Uri(prefix + "appbar.text.sans.png");
						break;
					case "StaticWebView":
						uri = new Uri(prefix + "appbar.browser.wire.png");
						break;
					case "SingleUrlImage":
						uri = new Uri(prefix + "appbar.image.png");
						break;
					case "Countdown":
						uri = new Uri(prefix + "appbar.timer.png");
						break;
					case "SimpleListPage":
						uri = new Uri(prefix + "appbar.lines.horizontal.4.png");
						break;
					case "RssList":
						uri = new Uri(prefix + "appbar.rss.png");
						break;
					case "FacebookLike":
						uri = new Uri(prefix + "appbar.thumbs.up.png");
						break;
				}
			}
			catch (Exception)
			{
				uri = new Uri(prefix + "appbar.warning.png");
			}
			return new BitmapImage(uri);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}