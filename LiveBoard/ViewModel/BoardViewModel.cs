using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;
using LiveBoard.Model;
using Newtonsoft.Json;

namespace LiveBoard.ViewModel
{
	public class BoardViewModel: ViewModelBase
	{
		public void Clear()
		{
			Board = new Board();
		}

		public void Start()
		{
			CurrentIndex = -1;
			MoveNext(); // 여기서 Index가 ++을 통해 0이 됨.
		}

		/// <summary>
		/// 다음 슬라이드로.
		/// </summary>
		/// <returns></returns>
		public int MoveNext()
		{
			++CurrentIndex;
			if (CurrentIndex >= Board.Pages.Count)
			{
				CurrentIndex = 0;
				if (!Board.IsLoop)
					throw new IndexOutOfRangeException("BoardViewModel.MoveNext");
			}
			return CurrentIndex;
		}

		/// <summary>
		/// 현재 인덱스
		/// </summary>
		[JsonIgnore]
		public int CurrentIndex
		{
			get { return _currentIndex; }
			private set
			{
				_currentIndex = value;
				RaisePropertyChanged("CurrentIndex");
			}
		}

		/// <summary>
		/// Target board.
		/// </summary>
		public Board Board
		{
			get { return _board; }
			set
			{
				_board = value;
				RaisePropertyChanged("Board");
			}
		}

		public void Stop()
		{
			CurrentIndex = 0;
		}

		/// <summary>
		/// JSON 으로 시리얼라이즈.
		/// 저장용.
		/// </summary>
		/// <returns></returns>
		public Task<string> SaveAsync()
		{
			return JsonConvert.SerializeObjectAsync(Board);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="json"></param>
		/// <returns></returns>
		public async Task LoadAsync(string json)
		{
			Board = await JsonConvert.DeserializeObjectAsync<Board>(json);
			RaisePropertyChanged("Board");
		}

		private int _currentIndex;
		private Board _board = new Board();

	}
}
