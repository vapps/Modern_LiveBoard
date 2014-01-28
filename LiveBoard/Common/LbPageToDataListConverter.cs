using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using LiveBoard.Model;
using LiveBoard.PageTemplate.Model;
using LiveBoard.ViewModel;

namespace LiveBoard.Common
{
	public class LbPageToDataListConverter: IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var page = value as IPage;
			if (page == null)
				return null;

			var list = page.Data;
			if (list == null)
				return null;
			return list;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotImplementedException();
		}
	}
}
