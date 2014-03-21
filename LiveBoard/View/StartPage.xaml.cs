using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.System.UserProfile;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;
using LiveBoard.ViewModel;

namespace LiveBoard.View
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class StartPage : Page
	{
		// 언어 리소스 로더.
		readonly ResourceLoader _loader = new ResourceLoader("Resources");

		readonly MainViewModel _viewModel;
		private NavigationHelper navigationHelper;
		StorageFile _playStorageFile;
		private FileActivatedEventArgs _fileEventArgs = null;
		private ProtocolActivatedEventArgs _protocolEventArgs = null;

		/// <summary>
		/// NavigationHelper is used on each page to aid in navigation and 
		/// process lifetime management
		/// </summary>
		public NavigationHelper NavigationHelper
		{
			get { return this.navigationHelper; }
		}

		public StartPage()
		{
			this.InitializeComponent();
			this.navigationHelper = new NavigationHelper(this);
			this.navigationHelper.LoadState += navigationHelper_LoadState;
			this.navigationHelper.SaveState += navigationHelper_SaveState;

			if (_viewModel == null)
				_viewModel = DataContext as MainViewModel;
		}

		private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
		{
		}

		private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
		{
			var boardViewModel = e.NavigationParameter as BoardViewModel;
			if (boardViewModel != null)
				Frame.Navigate(typeof(CreatePage), boardViewModel);
		}

		/// The methods provided in this section are simply used to allow
		/// NavigationHelper to respond to the page's navigation methods.
		/// 
		/// Page specific logic should be placed in event handlers for the  
		/// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
		/// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
		/// The navigation parameter is available in the LoadState method 
		/// in addition to page state preserved during an earlier session.
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			navigationHelper.OnNavigatedFrom(e);
		}

		/// <summary>
		/// lvbd 파일을 더블클릭하여 실행되었을 때 사용되는 파일 프로퍼티.
		/// </summary>
		public FileActivatedEventArgs FileEvent
		{
			get { return _fileEventArgs; }
			set { _fileEventArgs = value; }
		}

		public ProtocolActivatedEventArgs ProtocolEvent
		{
			get { return _protocolEventArgs; }
			set { _protocolEventArgs = value; }
		}

		/// <summary>
		/// lvbd 파일을 더블클릭하여 실행되었을 때.
		/// </summary>
		public async Task NavigateToFilePage()
		{
			// Display the result of the file activation if we got here as a result of being activated for a file.
			if (FileEvent != null && FileEvent.Files.Count > 0)
			{
				await LoadFileAsync(FileEvent.Files[0] as StorageFile);
			}
		}

		public void NavigateToProtocolPage()
		{

		}

		private async void ButtonCreate_OnClick(object sender, RoutedEventArgs e)
		{
			_viewModel.SelectedBoard = null;
			_viewModel.ActiveBoard = new BoardViewModel();
			_viewModel.ActiveBoard.Clear();
			_viewModel.ActiveBoard.Board.Author = await getAuthorNameAsync();
			Frame.Navigate(typeof(CreatePage));
		}

		/// <summary>
		/// PC 로그인한 사용자 이름 반환.
		/// </summary>
		/// <returns></returns>
		private async Task<string> getAuthorNameAsync()
		{
			string displayName = await UserInformation.GetDisplayNameAsync();

			if (string.IsNullOrEmpty(displayName))
			{
				var firstName = await UserInformation.GetFirstNameAsync();
				var lastName = await UserInformation.GetLastNameAsync();
				var name = firstName + (!String.IsNullOrEmpty(firstName) ? " " : "") + lastName;
				return name.Trim();
			}
			return displayName;
		}

		private async void ButtonOpen_OnClick(object sender, RoutedEventArgs e)
		{
			//if (!EnsureUnsnapped())
			//	return;

			var openPicker = new FileOpenPicker
			{
				ViewMode = PickerViewMode.List,
				SuggestedStartLocation = PickerLocationId.DocumentsLibrary
			};
			openPicker.FileTypeFilter.Add(".lbd");
			openPicker.FileTypeFilter.Add(".lvbd");

			StorageFile file = await openPicker.PickSingleFileAsync();

			await LoadFileAsync(file);
		}

		/// <summary>
		/// 불러오기.
		/// </summary>
		public async Task LoadFileAsync(StorageFile file)
		{
			if (file == null)
				return;

			if (_viewModel.ActiveBoard == null)
				_viewModel.ActiveBoard = new BoardViewModel();
			if (_viewModel.SelectedBoard == null)
				_viewModel.SelectedBoard = new BoardViewModel();

			await _viewModel.SelectedBoard.LoadAsync(file, _viewModel.Templates);

			// 최근 문서로 저장.
			// http://msdn.microsoft.com/en-us/library/windows/apps/hh972344.aspx
			StorageApplicationPermissions.MostRecentlyUsedList.Add(file, file.Path);
			Frame.Navigate(typeof(CreatePage), _viewModel.SelectedBoard);
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

		private async void ButtonPlayRecent_OnClick(object sender, RoutedEventArgs e)
		{
			if (_viewModel == null
				|| StorageApplicationPermissions.MostRecentlyUsedList.Entries == null
				|| StorageApplicationPermissions.MostRecentlyUsedList.Entries.Count == 0)
			{
				await new MessageDialog(_loader.GetString("NoRecentFile")).ShowAsync();
				return;
			}

			String mruFirstToken = null;
			try
			{
				mruFirstToken = StorageApplicationPermissions.MostRecentlyUsedList.Entries.First().Token;
				_playStorageFile = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(mruFirstToken);
			}
			catch (IOException)
			{
				if (!String.IsNullOrEmpty(mruFirstToken))
				{
					var converter = new ExtractFilenameConverter();
					var filename = converter.Convert(mruFirstToken, typeof (string), null, GlobalizationPreferences.Languages[0]);
					new MessageDialog(String.Format(_loader.GetString("ErrorFileLoading"), filename),
						_loader.GetString("PlayRecentBoard/Text")).ShowAsync();

					// 최근 실행목록 삭제.
					StorageApplicationPermissions.MostRecentlyUsedList.Remove(mruFirstToken);
					_viewModel.RefreshRecentOpenedList();
					return;
				}
			}

			var dialog = new MessageDialog(String.Format(_loader.GetString("AskToPlayFile"), _playStorageFile.DisplayName), _loader.GetString("PlayRecentBoard/Text"));
			dialog.Commands.Add(new UICommand(_loader.GetString("PlayNow"), playClickHandler));
			dialog.Commands.Add(new UICommand(_loader.GetString("Cancel/Text")));
			dialog.DefaultCommandIndex = 0;
			dialog.CancelCommandIndex = 1;
			await dialog.ShowAsync();
		}

		private async void playClickHandler(IUICommand command)
		{
			if (_playStorageFile != null)
			{
				await _viewModel.ActiveBoard.LoadAsync(_playStorageFile, _viewModel.Templates);
				this.Frame.Navigate(typeof(ShowPage), _viewModel.ActiveBoard);
			}
		}

		private async void ButtonEditRecent_OnClick(object sender, RoutedEventArgs e)
		{
			if (_viewModel == null
				|| StorageApplicationPermissions.MostRecentlyUsedList.Entries == null
				|| StorageApplicationPermissions.MostRecentlyUsedList.Entries.Count == 0)
			{
				await new MessageDialog(_loader.GetString("NoRecentFile")).ShowAsync();
				return;
			}

			Frame.Navigate(typeof(RecentOpenedPage));
		}

		/// <summary>
		/// 해상도 대응
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void RootPage_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (e.NewSize.Width <= (1366/2))
			{
				VisualStateManager.GoToState(this, "MinimalLayout", true);
			}
			else
			{
				VisualStateManager.GoToState(this, "DefaultLayout", true);
			} 
		}
	}
}
