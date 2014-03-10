using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Xml.Linq;
using Windows.Storage;
using Windows.Storage.AccessCache;
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
using LiveBoard.PageTemplate.Model;

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
		readonly DispatcherTimer _timer;
		private bool _currentPageStarted;
		private string _popupMessage;
		private TemplateListViewModel _templates;
		private ObservableCollection<string> _recentBoards;
		private int _maxRecentBoardCount = 5;
		private RecentOpenedListViewModel _recentOpenedList;
		private BoardViewModel _selectedBoard = new BoardViewModel();

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
					case LbMessageType.EVT_PAGE_CREATING:
						var page = generatePageFromTemaplate(message.Content.Data as LbTemplate);
						ActiveBoard.Board.Pages.Add(page);
						break;
				}
			});

			// 초기화하면서 TemplateList.xml 파일을 로딩한다.
			Templates = new TemplateListViewModel();
		}

		#region ICommand
		public ICommand SaveCmd { get { return new RelayCommand<BoardViewModel>(Save); } }
		public ICommand AddPageCmd { get { return new RelayCommand(AddPage); } }
		public ICommand DeletePageCmd { get { return new RelayCommand<Object>(DeletePage, CanDeletePage); } }
		public ICommand PlayCmd { get { return new RelayCommand<BoardViewModel>(Play); } }
		public ICommand PreviewCmd { get { return new RelayCommand<BoardViewModel>(Preview); } }
		public ICommand StopCmd { get { return new RelayCommand<BoardViewModel>(Stop); } }
		#endregion ICommand

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
			if (board == null)
				board = ActiveBoard;
			ActiveBoard = board;

			if (!IsPlaying)
				IsPlaying = true;
			else
			{
				Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
				{
					MessageType = LbMessageType.ERROR,
					Data = LbError.IsPlayingTrue,
					Board = board.Board
				}));
				return;
			}

			Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
			{
				MessageType = LbMessageType.EVT_SHOW_STARTING,
				Data = ActiveBoard,
				Board = board.Board
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

			--CurrentRemainedSecond;
			if (CurrentRemainedSecond < 0)
			{
				_timer.Stop();
				try
				{
					if (IsPlaying)
					{
						CurrentPage = ActiveBoard.Board.Pages[ActiveBoard.MoveNext()];
					}
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

			CurrentPageElapsedRatio = (int)(((CurrentPage.Duration.TotalSeconds - CurrentRemainedSecond) / CurrentPage.Duration.TotalSeconds) * 100);

			Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
			{
				MessageType = LbMessageType.EVT_TICK
			}));
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
		public async void Save(BoardViewModel boardViewModel)
		{
			if (!EnsureUnsnapped())
				return;

			if (boardViewModel == null)
				boardViewModel = ActiveBoard;

			var savePicker = new FileSavePicker { SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
			// Dropdown of file types the user can save the file as
			savePicker.FileTypeChoices.Add("LiveBoard file", new List<string>() { ".lvbd" });
			// Default file name if the user does not type one in or select a file to replace
			savePicker.SuggestedFileName = BoardViewModel.CreateNewFilename();
			savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
			StorageFile file = await savePicker.PickSaveFileAsync();
			if (file == null)
			{
				PopupMessage = "Operation cancelled.";
			}
			else
			{
				// Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
				CachedFileManager.DeferUpdates(file);
				// write to file
				String content = boardViewModel.Board.ToXml().ToString();

				await FileIO.WriteTextAsync(file, content, UnicodeEncoding.Utf8);
				// Let Windows know that we're finished changing the file so the other app can update the remote version of the file.
				// Completing updates may require Windows to ask for user input.
				FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
				StorageApplicationPermissions.MostRecentlyUsedList.Add(file, file.Path);

				// TODO: 완료 팝업. 아직 안함.
				if (status == FileUpdateStatus.Complete)
				{
					PopupMessage = "File " + file.Name + " was saved.";
				}
				else
				{
					PopupMessage = "File " + file.Name + " couldn't be saved.";
				}
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
			// 템플릿오브젝트에서 페이지 생성.
			var t = new LbTemplate
			{
				Key = "SingleUrlImage",
				DisplayName = "Simple image viewer from web",
				Description = "Show single image",
				TemplateView = "SimpleUrlImage",
				TemplateModel = "SingleStringPage",
				DataList = new List<LbPageData>()
				{
					new LbPageData()
					{
						Key = "Url", 
						DefaultData = "http://inserbia.info/news/wp-content/uploads/2013/05/grizzly-650x487.jpg",
						Name = "헤더 정보",
						ValueType = typeof(String)
					}
				}
			};

			var pageExample1 = generatePageFromTemaplate(t);
			ActiveBoard.Board.Pages.Add(pageExample1);

			//var page4 = new SingleStringPage
			//{
			//	TemplateKey = "Countdown",
			//	Title = "타이틀 " + DateTime.Now.Ticks,
			//	Duration = TimeSpan.FromSeconds(6.0d),
			//	IsVisible = true,
			//	Guid = Guid.NewGuid().ToString(),
			//	View = "OneNumberCount",
			//	Data = new List<LbPageData>(){
			//		new LbPageData()
			//			{
			//				Key = "Number",
			//				Data = 5,
			//				Name = "헤더 정보",
			//				ValueType = typeof(int)
			//			}
			//	}
			//};

			//ActiveBoard.Board.Pages.Add(page4);

			//var page = new SingleStringPage
			//{
			//	TemplateKey = "StaticWebView",
			//	Title = "타이틀",
			//	Duration = TimeSpan.FromSeconds(7.0d),
			//	IsVisible = true,
			//	Guid = Guid.NewGuid().ToString(),
			//	View = "StaticWebView",
			//	Data = new List<LbPageData>(){
			//		new LbPageData()
			//			{
			//				Key = "URL",
			//				Data = "http://www.naver.com",
			//				Name = "인터넷 주소",
			//				ValueType = typeof(string)
			//			}
			//	}
			//};

			//ActiveBoard.Board.Pages.Add(page);

			//var page2 = new RssList()
			//{
			//	TemplateKey = "RssList",
			//	Title = "타이틀 " + DateTime.Now.Ticks.ToString(),
			//	Duration = TimeSpan.FromSeconds(5.0d),
			//	IsVisible = true,
			//	Guid = Guid.NewGuid().ToString(),
			//	View = "SimpleList",
			//	Data = new List<LbPageData>()
			//	{
			//		new LbPageData()
			//		{
			//			Key = "Header",
			//			Name = "타이틀바",
			//			ValueType = typeof(string),
			//			Data = "다음 View 인기 기사"
			//		},
			//		new LbPageData()
			//		{
			//			Key="RSS",
			//			Name="RSS 주소",
			//			ValueType = typeof(string),
			//			Data = "http://v.daum.net/best/rss"
			//		},
			//		new LbPageData()
			//		{
			//			Key="Feeds",
			//			Name="출력될 Feed 목록",
			//			ValueType = typeof(IEnumerable<string>),
			//			IsHidden = true
			//		}
			//	}
			//};
			//ActiveBoard.Board.Pages.Add(page2);

		}

		/// <summary>
		/// 데이터로 템플릿 사용.
		/// </summary>
		/// <param name="template"></param>
		/// <returns></returns>
		private IPage generatePageFromTemaplate(LbTemplate template)
		{
			if (template == null)
				throw new ArgumentNullException("template");

			var model = Type.GetType("LiveBoard.PageTemplate.Model." + template.TemplateModel);
			if (model == null)
				throw new ArgumentException("Template model not found.");

			var page = (IPage)Activator.CreateInstance(model);
			page.TemplateKey = template.Key;
			page.View = template.TemplateView;
			page.Title = "Page " + (ActiveBoard.Board.Pages.Count + 1);
			page.Duration = TimeSpan.FromSeconds(5.0d);
			page.IsVisible = true;
			page.Guid = Guid.NewGuid().ToString();
			page.Data = template.DataList;

			return page;
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
			else if (obj is IPage)
			{
				ActiveBoard.Board.Pages.Remove((IPage)obj);
			}
		}

		private bool CanDeletePage(Object arg)
		{
			return arg != null && (!(arg is IEnumerable<IPage>) || !(arg is IPage));
		}

		/// <summary>
		/// 템플릿 목록.
		/// </summary>
		public TemplateListViewModel Templates
		{
			get { return _templates; }
			set
			{
				_templates = value;
				RaisePropertyChanged("Templates");
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

				if (_currentPage != null)
					_currentPage.PrepareToLoadAsync(); // Command to Load if it needs to be prepared.

				// Send events.
				MessengerInstance.Send(new GenericMessage<LbMessage>(this, new LbMessage()
				{
					MessageType = LbMessageType.EVT_PAGE_READY,
					Data = CurrentPage
				}));
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
		/// 선택한 보드 뷰모델
		/// </summary>
		public BoardViewModel SelectedBoard
		{
			get { return _selectedBoard; }
			set
			{
				_selectedBoard = value;
				RaisePropertyChanged("SelectedBoard");
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

		public RecentOpenedListViewModel RecentOpenedList
		{
			get
			{
				if (_recentOpenedList == null)
					_recentOpenedList = new RecentOpenedListViewModel();

				_recentOpenedList.Clear();
				foreach (var entry in StorageApplicationPermissions.MostRecentlyUsedList.Entries)
				{
					_recentOpenedList.Add(new LbFile()
					{
						Token = entry.Token,
						Metadata = entry.Metadata
					});
					Debug.WriteLine(entry.Metadata);
				}

				return _recentOpenedList;
			}
			set
			{
				_recentOpenedList = value;
				RaisePropertyChanged("RecentOpenedList");
			}
		}

		#endregion Properties

	}
}