using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using LiveBoard.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LiveBoard.ViewModel
{
	public class BoardViewModel : ViewModelBase
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
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <param name="templates"></param>
		/// <returns></returns>
		public async Task LoadAsync(string text, IEnumerable<LbTemplate> templates)
		{
			await Task.Run(() =>
			{
				Board = Board.FromXml(XElement.Parse(text), templates);
			});
		}

		private int _currentIndex;
		private Board _board = new Board();

	}
}
