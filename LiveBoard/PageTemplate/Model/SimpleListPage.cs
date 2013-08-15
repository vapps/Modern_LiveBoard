using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using LiveBoard.Model;

namespace LiveBoard.PageTemplate.Model
{
	public class SimpleListPage : ObservableObject, IPage
	{
		public string Guid { get; set; }
		public string Title { get; set; }
		public string TemplateCode { get; set; }
		public string TemplateOption { get; set; }
		public TimeSpan Duration { get; set; }
		public string Description { get; set; }
		public bool IsVisible { get; set; }
		public virtual async Task<bool> PrepareToLoadAsync()
		{
			// do nothing
			return true;
		}


		/// <summary>
		/// Specific data.
		/// </summary>
		public virtual ListData Data
		{
			get { return _data; }
			set
			{
				_data = value;
				RaisePropertyChanged("Data");
			}
		}

		public class ListData : ObservableObject
		{
			private ObservableCollection<string> _stringList;
			private string _header;

			public string Header
			{
				get { return _header; }
				set
				{
					_header = value;
					RaisePropertyChanged("Header");
				}
			}

			public ObservableCollection<string> StringList
			{
				get { return _stringList; }
				set
				{
					_stringList = value;
					RaisePropertyChanged("StringList");
				}
			}
		}

		private ListData _data;

	}
}