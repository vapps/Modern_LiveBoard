using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace LiveBoard.PageTemplate.Model
{
	public class SimpleListPage : ObservableObject, IPage
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
		public string TemplateOption { get; set; }

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
			// do nothing
			return true;
		}


		/// <summary>
		/// Specific data.
		/// </summary>
		public virtual object Data
		{
			get { return _data; }
			set
			{
				_data = value;
				RaisePropertyChanged("Data");
			}
		}

		private object _data;
		private string _title;
		private TimeSpan _duration;
		private string _description;
	}
}