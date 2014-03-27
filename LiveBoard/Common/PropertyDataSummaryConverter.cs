using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using LiveBoard.PageTemplate.Model;

namespace LiveBoard.Common
{
	/// <summary>
	/// 데이터 타입에 따라 요약을 보여준다.
	/// <see cref="Visibility.Collapsed"/>.
	/// </summary>
	public sealed class PropertyDataSummaryConverter : IValueConverter
	{
		readonly ResourceLoader _loader = new ResourceLoader("Resources");

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var data = value as IEnumerable<LbPageData>;
			if (data != null)
			{
				var lbPageDatas = data as IList<LbPageData> ?? data.ToList();
				if (lbPageDatas.Count() > 1)
					return String.Format(_loader.GetString("MultipleProperties"), lbPageDatas.Count(p => p.IsHidden == false));
				return String.Format("{0}: {1}", (lbPageDatas.ElementAt(0)).Name, (lbPageDatas.ElementAt(0)).Data);
			}
			return value;
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
