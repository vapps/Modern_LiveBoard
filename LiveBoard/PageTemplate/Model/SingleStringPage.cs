using System;
using System.Threading.Tasks;
using Windows.Web.Syndication;
using GalaSoft.MvvmLight;
using LiveBoard.Model;

namespace LiveBoard.PageTemplate.Model
{
	/// <summary>
	/// 페이지 인터페이스
	/// </summary>
	public class SingleStringPage : ObservableObject, IPage
	{
		public string Guid { get; set; }
		public string Title { get; set; }
		public string View { get; set; }
		public string ViewOption { get; set; }
		public string TemplateOption { get; set; }
		public TimeSpan Duration { get; set; }
		public string Description { get; set; }
		public bool IsVisible { get; set; }
		public async Task<bool> PrepareToLoadAsync()
		{
			// do nothing.
			return true;
		}

		/// <summary>
		/// Specific data.
		/// </summary>
		public object Data
		{
			get { return _data; }
			set
			{
				_data = value;
				RaisePropertyChanged("Data");
			}
		}

		public override string ToString()
		{
			return String.Format("{0} ({1})", Title, View);
		}

		private object _data;
	}
}
