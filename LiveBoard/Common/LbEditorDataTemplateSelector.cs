﻿using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LiveBoard.PageTemplate.Model;

namespace LiveBoard.Common
{
	public class LbEditorDataTemplateSelector : DataTemplateSelector
	{
		public DataTemplate DefaultTemplate { get; set; }
		public DataTemplate IntTemplate { get; set; }
		public DataTemplate StringTemplate { get; set; }
		public DataTemplate DoubleTemplate { get; set; }
		public DataTemplate ColorTemplate { get; set; }
		public DataTemplate HiddenTemplate { get; set; }

		protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
		{
			var data = item as LbPageData;
			if (data == null)
				return DefaultTemplate;

			if (data.IsHidden)
				return HiddenTemplate;

			if (data.ValueType == typeof(int))
				return IntTemplate;
			if (data.ValueType == typeof(string))
				return StringTemplate;
			if (data.ValueType == typeof(double))
				return DoubleTemplate; 
			if (data.ValueType == typeof(Color))
				return ColorTemplate;

			return DefaultTemplate;
		}

	}
}
