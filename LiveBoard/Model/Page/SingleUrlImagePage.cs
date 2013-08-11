using System;
using GalaSoft.MvvmLight;

namespace LiveBoard.Model.Page
{
	public class SingleUrlImagePage : ObservableObject, IPage
	{
		public string Guid { get; set; }
		public string Title { get; set; }
		public string TemplateCode { get; set; }
		public string TemplateOption { get; set; }
		public TimeSpan Duration { get; set; }
		public string Description { get; set; }
		public bool IsVisible { get; set; }


		/// <summary>
		/// Specific data.
		/// </summary>
		public string Data
		{
			get { return _data; }
			set
			{
				_data = value;
				RaisePropertyChanged("Data");
			}
		}
		private string _data;

	}
}