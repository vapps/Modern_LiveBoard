using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace LiveBoard.Model
{
	/// <summary>
	/// 파일 액세스를 위한 정보 클래스.
	/// </summary>
	public class LbFile: ObservableObject
	{
		private string _token;
		private string _metadata;

		/// <summary>
		/// 파일 액세스 토큰.
		/// </summary>
		public string Token
		{
			get { return _token; }
			set
			{
				_token = value;
				RaisePropertyChanged("Token");
			}
		}

		/// <summary>
		/// 부가정보.
		/// </summary>
		public string Metadata
		{
			get { return _metadata; }
			set
			{
				_metadata = value; 
				RaisePropertyChanged("Metadata");
			}
		}
	}
}
