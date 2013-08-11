using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;
using LiveBoard.Model;
using LiveBoard.Model.Page;

namespace LiveBoard.ViewModel
{
	/// <summary>
	/// 메인 뷰모델
	/// </summary>
	public class MainViewModel : ViewModelBase
	{
		private BoardViewModel _activeBoard = new BoardViewModel();
		private bool _isPreview;
		private bool _isPlaying;
		private DateTime _startTime;
		private IPage _currentPage;
		private int _currentPageElapsedRatio;
		private int _currentRemainedSecond;
		DispatcherTimer _timer;
		private bool _currentPageStarted;
		private string _popupMessage;

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel()
		{
			////if (IsInDesignMode)
			////{
			////    // Code runs in Blend --> create design time data.
			////}
			////else
			////{
			////    // Code runs "for real"
			////}

			/* 쇼 동작 원리: 시작
			 * 1. 로딩.
			 * 2. MainViewModel.Play는 ActiveBoard.Start를 호출하고 이를 통해 첫 페이지 로딩 후 EVT_STARTING 전송. Starting으로 ShowPage Navigate.
			 * 3. ShowPage에서 이 메시지로 ActiveBoard의 CurrentPage를 로딩하고 그에 맞는 XAML 로딩.
			 * 4. 내부에 XAML이 EVT_LOADED 로딩.
			 * 5. 그에 대한 메시지로 현재 페이지에 대한 RemainedSecond를 셋팅.
			 * 6. tick마다 --RemainedSecond.
			 * 7. RemainedSecond < 0  이 되면 MoveNext..
			 * 
			 * 총 메시지는 3번: ready, loaded, start.
			*/
			/* 쇼 동작원리: 종료
			 * 1. MainViewModel에서 타이머 종료, 첫페이지 돌리기, Finishing 명령 전송.
			 * 2. ShowPage 에서 이를 통해 Finished 및 Page Close.
			 * */

			_timer = new DispatcherTimer();
			_timer.Tick += PlayTimerEventHandler;
			_timer.Interval = new TimeSpan(0, 0, 1);

			Messenger.Default.Register<GenericMessage<LbMessage>>(this, message =>
			{
				Debug.WriteLine("* MainViewModel Received Message: " + message.Content.MessageType.ToString());
				switch (message.Content.MessageType)
				{
					case LbMessageType.EVT_SHOW_FINISHING:
						Stop(ActiveBoard);
						break;
					case LbMessageType.EVT_PAGE_STARTED:
						// 페이지 장착 완료.
						CurrentRemainedSecond = (int)CurrentPage.Duration.TotalSeconds;
						CurrentPageStarted = true;

						_timer.Start();

						break;
					case LbMessageType.EVT_SHOW_STARTED:
						// 첫페이지 로딩.
						ActiveBoard.Start();
						CurrentPage = ActiveBoard.Board.Pages[ActiveBoard.CurrentIndex];
						break;
				}
			});
		}
		#region ICommand
		public ICommand LoadCmd { get { return new RelayCommand(Load); } }
		public ICommand SaveCmd { get { return new RelayCommand(Save); } }
		public ICommand AddPageCmd { get { return new RelayCommand(AddPage); } }
		public ICommand DeletePageCmd { get { return new RelayCommand<IPage>(DeletePage); } }
		public ICommand PlayCmd { get { return new RelayCommand<BoardViewModel>(Play); } }
		public ICommand PreviewCmd { get { return new RelayCommand<BoardViewModel>(Preview); } }
		public ICommand StopCmd { get { return new RelayCommand<BoardViewModel>(Stop); } }
		#endregion ICommand

		/// <summary>
		/// 불러오기.
		/// </summary>
		public async void Load()
		{
			if (!EnsureUnsnapped())
				return;

			var openPicker = new FileOpenPicker
			{
				ViewMode = PickerViewMode.List,
				SuggestedStartLocation = PickerLocationId.DocumentsLibrary
			};
			openPicker.FileTypeFilter.Add(".lbd");

			StorageFile file = await openPicker.PickSingleFileAsync();
			if (file == null)
				return;

			if (ActiveBoard == null)
				ActiveBoard = new BoardViewModel();

			var text = await FileIO.ReadTextAsync(file);
			await ActiveBoard.LoadAsync(text);
			RaisePropertyChanged("ActiveBoard"); // Force UI change
		}

		/// <summary>
		/// 종료
		/// </summary>
		/// <param name="obj"></param>
		private void Stop(BoardViewModel obj)
		{
			_timer.Stop();
			ActiveBoard.Stop();
			IsPlaying = false;
			IsPreview = false;
			Messenger.Default.Send(new GenericMessage<LbMessage>(new LbMessage()
			{
				MessageType = LbMessageType.EVT_SHOW_FINISHED
			}));
		}

		public void Preview(BoardViewModel board)
		{
			IsPreview = true;
			Play(board);
		}

		/// <summary>
		/// 재생하기
		/// </summary>
		/// <param name="board"></param>
		public void Play(BoardViewModel board)
		{
			if (!IsPlaying)
				IsPlaying = true;
			else
			{
				Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
				{
					MessageType = LbMessageType.ERROR,
					Data = LbError.IsPlayingTrue
				}));
				return;
			}

			Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
			{
				MessageType = LbMessageType.EVT_SHOW_STARTING,
				Data = ActiveBoard
			}));

			StartTime = DateTime.Now;

		}


		/// <summary>
		/// 매 초마다 실행되는 초단위 이벤트 핸들러
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PlayTimerEventHandler(object sender, object e)
		{
			Debug.WriteLine("tick at {0} and Elapsed {1}", DateTime.Now.ToString("u"), (DateTime.Now - StartTime).ToString("g"));

			// TODO: 로직이 들어가야 한다.
			if (!IsPlaying)
				return;

			--CurrentRemainedSecond;
			if (CurrentRemainedSecond < 0)
			{
				_timer.Stop();
				try
				{
					CurrentPage = ActiveBoard.Board.Pages[ActiveBoard.MoveNext()];
					Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
					{
						MessageType = LbMessageType.EVT_PAGE_FINISHING
					}));
				}
				catch (IndexOutOfRangeException)
				{
					Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
					{
						MessageType = LbMessageType.EVT_SHOW_FINISHING
					}));
				}
			}

			CurrentPageElapsedRatio = (int) (((CurrentPage.Duration.TotalSeconds - CurrentRemainedSecond) / CurrentPage.Duration.TotalSeconds)*100);

			//Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
			//{
			//	MessageType = LbMessageType.EVT_TICK
			//}));
		}

		internal bool EnsureUnsnapped()
		{
			// FilePicker APIs will not work if the application is in a snapped state.
			// If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
			bool unsnapped = ((ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap());
			if (!unsnapped)
			{
				Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
				{
					MessageType = LbMessageType.ERROR,
					Data = LbError.UnSnappedToSave
				}));
			}

			return unsnapped;
		}

		/// <summary>
		/// 저장하기.
		/// </summary>
		public async void Save()
		{
			if (!EnsureUnsnapped())
				return;
			var savePicker = new FileSavePicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
			// Dropdown of file types the user can save the file as
			savePicker.FileTypeChoices.Add("LiveBoard file", new List<string>() { ".lbd" });
			// Default file name if the user does not type one in or select a file to replace
			savePicker.SuggestedFileName = "LiveBoard " + DateTime.Now.ToString("s").Replace(':', '_');
			savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			StorageFile file = await savePicker.PickSaveFileAsync();
			if (file != null)
			{
				// Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
				CachedFileManager.DeferUpdates(file);
				// write to file
				String content = await ActiveBoard.SaveAsync();

				await FileIO.WriteTextAsync(file, content, UnicodeEncoding.Utf8);
				// Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
				// Completing updates may require Windows to ask for user input.
				FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
				if (status == FileUpdateStatus.Complete)
				{
					PopupMessage = "File " + file.Name + " was saved.";
				}
				else
				{
					PopupMessage = "File " + file.Name + " couldn't be saved.";
				}
			}
			else
			{
				PopupMessage = "Operation cancelled.";
			}
		}

		public string PopupMessage
		{
			get { return _popupMessage; }
			set
			{
				_popupMessage = value;
				RaisePropertyChanged("PopupMessage");
			}
		}

		/// <summary>
		/// 페이지 추가
		/// </summary>
		private void AddPage()
		{
			var page = new SingleTextPage
			{
				Title = "http://inserbia.info/news/wp-content/uploads/2013/05/grizzly-650x487.jpg", //"타이틀 " + DateTime.Now.Ticks.ToString(),
				Duration = TimeSpan.FromSeconds(5.0d),
				IsVisible = true,
				Guid = Guid.NewGuid().ToString(),
				TemplateCode = "BlankPage_SingleUrlImage"
			};
			ActiveBoard.Board.Pages.Add(page);
		}

		/// <summary>
		/// 페이지(들) 삭제.
		/// </summary>
		/// <param name="obj">삭제 데이터. 여러페이지일 경우 <![CDATA[IEnumerable<IPage>]]> 또는 하나의 페이지일 경우 <see cref="IPage"/> 오브젝트.</param>
		private void DeletePage(Object obj)
		{
			if (obj is IEnumerable<IPage>)
			{
				foreach (var page in (IEnumerable<IPage>)obj)
					ActiveBoard.Board.Pages.Remove(page);
			}

			if (obj is IPage)
			{
				ActiveBoard.Board.Pages.Remove((IPage)obj);
			}
		}

		#region Properties

		/// <summary>
		/// 현재 페이지
		/// </summary>
		public IPage CurrentPage
		{
			get { return _currentPage; }
			set
			{
				_currentPage = value;
				RaisePropertyChanged("CurrentPage");
			}
		}

		public bool CurrentPageStarted
		{
			get { return _currentPageStarted; }
			set
			{
				_currentPageStarted = value;
				RaisePropertyChanged("CurrentPageStarted");
			}
		}

		/// <summary>
		/// 현재 슬라이드의 남은 시간
		/// </summary>
		public int CurrentRemainedSecond
		{
			get { return _currentRemainedSecond; }
			set
			{
				_currentRemainedSecond = value;
				RaisePropertyChanged("CurrentRemainedSecond");
			}
		}


		/// <summary>
		/// 현재 보드.
		/// </summary>
		public BoardViewModel ActiveBoard
		{
			get { return _activeBoard; }
			set
			{
				_activeBoard = value;
				RaisePropertyChanged("ActiveBoard");
			}
		}


		/// <summary>
		/// 미리보기 모드
		/// </summary>
		public bool IsPreview
		{
			get { return _isPreview; }
			set
			{
				_isPreview = value;
				RaisePropertyChanged("IsPreview");
			}
		}

		/// <summary>
		/// 쇼 시작 시간.
		/// </summary>
		public DateTime StartTime
		{
			get { return _startTime; }
			set
			{
				_startTime = value;
				RaisePropertyChanged("StartTime");
			}
		}

		/// <summary>
		/// 재생 중
		/// </summary>
		public bool IsPlaying
		{
			get { return _isPlaying; }
			set
			{
				_isPlaying = value;
				RaisePropertyChanged("IsPlaying");
			}
		}

		/// <summary>
		/// 해당 페이지 진행 시간. 0~100 value
		/// </summary>
		public int CurrentPageElapsedRatio
		{
			get { return _currentPageElapsedRatio; }
			set
			{
				_currentPageElapsedRatio = value;
				RaisePropertyChanged("CurrentPageElapsedRatio");
			}
		}

		#endregion Properties

	}
}