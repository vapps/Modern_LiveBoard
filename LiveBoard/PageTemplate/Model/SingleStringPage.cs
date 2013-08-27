using System;
using System.Threading.Tasks;
using Windows.Web.Syndication;
using GalaSoft.MvvmLight;

namespace LiveBoard.PageTemplate.Model
{
	/// <summary>
	/// 페이지 인터페이스
	/// </summary>
	public class SingleStringPage : ObservableObject, IPage
	{
		public string Guid { get; set; }

		public string Title
		{
			get { return _title; }
			set
			{
				_title = value; 
				RaisePropertyChanged("Title");
			}
		}

		public string View { get; set; }
		public string ViewOption { get; set; }

		public TimeSpan Duration
		{
			get { return _duration; }
			set
			{
				_duration = value; 
				RaisePropertyChanged("Duration");
			}
		}

		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				RaisePropertyChanged("Description");
			}
		}

		public bool IsVisible { get; set; }
		public string TemplateKey { get; set; }

		public virtual async Task<bool> PrepareToLoadAsync()
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
		private string _title;
		private TimeSpan _duration;
		private string _description;
	}
}
