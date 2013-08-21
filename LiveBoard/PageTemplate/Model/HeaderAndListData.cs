using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace LiveBoard.PageTemplate.Model
{
	/// <summary>
	/// 제목+목록형 데이터
	/// </summary>
	public class HeaderAndListData : ObservableObject
	{
		private ObservableCollection<string> _stringList = new ObservableCollection<string>();
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
}