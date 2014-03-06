using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Storage;
using GalaSoft.MvvmLight;
using LiveBoard.Model;
using Newtonsoft.Json;

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
				RaisePropertyChanged("RunningTime");
			}
		}

		/// <summary>
		/// 불러온 파일 경로.
		/// </summary>
		public StorageFile Filename
		{
			get { return _filename; }
			set
			{
				_filename = value;
				RaisePropertyChanged("Filename");
			}
		}

		public void Stop()
		{
			CurrentIndex = 0;
		}

		/// <summary>
		/// 총 실행시간.
		/// </summary>
		/// <remarks><see cref="Board"/>가 업데이트될 때 PropertyChanged로 업데이트 된다.</remarks>
		public TimeSpan RunningTime
		{
			get
			{
				TimeSpan totalMilliSecond;
				return Board.Pages.Aggregate(totalMilliSecond, (current, page) => current + page.Duration);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <param name="templates"></param>
		/// <returns></returns>
		public async Task LoadAsync(StorageFile file, IEnumerable<LbTemplate> templates)
		{
			var text = await FileIO.ReadTextAsync(file);
			Filename = file;
			Board = Board.FromXml(XElement.Parse(text), templates);
		}

		private int _currentIndex;
		private Board _board = new Board();
		private StorageFile _filename;
	}
}
