using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;
using LiveBoard.Model;

namespace LiveBoard.ViewModel
{
	public class BoardViewModel: ViewModelBase
	{
		private Board _board = new Board();

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

		private int _currentIndex;

		public void Stop()
		{
			CurrentIndex = 0;
		}
	}
}
