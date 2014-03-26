using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Core;
using Windows.UI.Popups;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234
using LiveBoard.Model;
using LiveBoard.PageTemplate.Model;
using LiveBoard.ViewModel;

namespace LiveBoard.View
{
	/// <summary>
	/// A page that displays a group title, a list of items within the group, and details for
	/// the currently selected item.
	/// </summary>
	public sealed partial class RecentOpenedPage : Page, INotifyPropertyChanged
	{
		private NavigationHelper navigationHelper;
		private MainViewModel _viewModel;
		ResourceLoader _loader = new Windows.ApplicationModel.Resources.ResourceLoader("Resources");

		/// <summary>
		/// NavigationHelper is used on each page to aid in navigation and 
		/// process lifetime management
		/// </summary>
		public NavigationHelper NavigationHelper
		{
			get { return this.navigationHelper; }
		}

		public RecentOpenedPage()
		{
			this.InitializeComponent();

			if (_viewModel == null)
				_viewModel = DataContext as MainViewModel;

			// Setup the navigation helper
			this.navigationHelper = new NavigationHelper(this);
			this.navigationHelper.LoadState += navigationHelper_LoadState;
			this.navigationHelper.SaveState += navigationHelper_SaveState;

			// Setup the logical page navigation components that allow
			// the page to only show one pane at a time.
			this.navigationHelper.GoBackCommand = new RelayCommand(() => this.GoBack(), () => this.CanGoBack());
			this.itemListView.SelectionChanged += itemListView_SelectionChanged;

			// Start listening for Window size changes 
			// to change from showing two panes to showing a single pane
			Window.Current.SizeChanged += Window_SizeChanged;
			this.InvalidateVisualState();


			// 메신저 연결.
			Messenger.Default.Register<GenericMessage<LbMessage>>(this, message =>
			{
				if (message.Content.MessageType == LbMessageType.EVT_SHOW_STARTING)
				{
					Debug.WriteLine("* RecentOpenedPage Received Message: " + message.Content.MessageType.ToString());
					var frame = (Frame)Window.Current.Content;
					if (!(frame.Content is ShowPage))
					{
						this.Frame.Navigate(typeof(ShowPage), message.Content.Data);
					}
				}
				else if (message.Content.MessageType == LbMessageType.EVT_PAGE_STARTED)
				{
					// 프리뷰의 페이지가 로딩되었을 때.
				}
			});

		}

		/// <summary>
		/// 파일 선택했을 때.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		async void itemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var lbFile = itemListView.SelectedItem as LbFile;
			if (lbFile == null)
				return;
			if (_viewModel.SelectedBoard == null)
				_viewModel.SelectedBoard = new BoardViewModel();

			// 파일 부르기.
			StorageFile retrievedFile = null;
			string text = null;
			try
			{
				retrievedFile = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(lbFile.Token);
				text = await FileIO.ReadTextAsync(retrievedFile);
			}
			catch (IOException)
			{
				var converter = new ExtractFilenameConverter();
				var filename = converter.Convert(lbFile.Metadata, typeof(string), null, Windows.System.UserProfile.GlobalizationPreferences.Languages[0]);
				new MessageDialog(String.Format(_loader.GetString("ErrorFileLoading"), filename), _loader.GetString("PlayRecentBoard/Text")).ShowAsync();

				// 최근 실행목록 삭제.
				StorageApplicationPermissions.MostRecentlyUsedList.Remove(lbFile.Token);
				_viewModel.RefreshRecentOpenedList();

				return;
			}

			Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
			{
				_viewModel.SelectedBoard.Board = Board.FromXml(XElement.Parse(text), _viewModel.Templates);
				_viewModel.SelectedBoard.Filename = retrievedFile;
			});

			if (this.UsingLogicalPageNavigation())
			{
				this.navigationHelper.GoBackCommand.RaiseCanExecuteChanged();
			}
		}

		/// <summary>
		/// Populates the page with content passed during navigation.  Any saved state is also
		/// provided when recreating a page from a prior session.
		/// </summary>
		/// <param name="sender">
		/// The source of the event; typically <see cref="NavigationHelper"/>
		/// </param>
		/// <param name="e">Event data that provides both the navigation parameter passed to
		/// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
		/// a dictionary of state preserved by this page during an earlier
		/// session.  The state will be null the first time a page is visited.</param>
		private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
		{
			if (itemListView.SelectedIndex == -1)
			{
				itemDetailGrid.Visibility = Visibility.Collapsed;
				SelectItemPleaseInstruction.Visibility = Visibility.Visible;
			}


			//if (e.PageState == null)
			//{
			//	// When this is a new page, select the first item automatically unless logical page
			//	// navigation is being used (see the logical page navigation #region below.)
			//	if (!this.UsingLogicalPageNavigation() && this.itemsViewSource.View != null)
			//	{
			//		this.itemsViewSource.View.MoveCurrentToFirst();
			//	}
			//}
			//else
			//{
			//	// Restore the previously saved state associated with this page
			//	if (e.PageState.ContainsKey("SelectedItem") && this.itemsViewSource.View != null)
			//	{
			//		// TODO: Invoke Me.itemsViewSource.View.MoveCurrentTo() with the selected
			//		//       item as specified by the value of pageState("SelectedItem")

			//	}
			//}
		}

		/// <summary>
		/// Preserves state associated with this page in case the application is suspended or the
		/// page is discarded from the navigation cache.  Values must conform to the serialization
		/// requirements of <see cref="SuspensionManager.SessionState"/>.
		/// </summary>
		/// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
		/// <param name="e">Event data that provides an empty dictionary to be populated with
		/// serializable state.</param>
		private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
		{
			//if (this.itemsViewSource.View != null)
			//{
			//	// TODO: Derive a serializable navigation parameter and assign it to
			//	//       pageState("SelectedItem")
			//}
		}

		/// <summary>
		/// XML 파일을 읽어서 필요 정보만 추출한다.
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		private async Task parseLiveBoardXml(LbFile file)
		{
			// 전체 페이지 수, 전체 실행시간 수를 계산하면 될 듯.
			StorageFile retrievedFile = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(file.Token);
			var text = await FileIO.ReadTextAsync(retrievedFile);
			var pages = new ObservableCollection<IPage>();
			var xElement = XElement.Parse(text);
			long totalMilliSecond = 0;
			int pageCount = 0;
			foreach (var page in xElement.Element("Pages").Elements("Page"))
			{
				++pageCount;
				totalMilliSecond += long.Parse(page.Attribute("Duration").Value);
			}
			var timeSpan = TimeSpan.FromMilliseconds(totalMilliSecond);
			var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

			//await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
			//{
			//	itemAuthor.Text = "Author: " + (!String.IsNullOrEmpty(xElement.Attribute("Author").Value) ? xElement.Attribute("Author").Value : "Unknown");
			//	itemRunningTime.Text = "Running Time: " + String.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			//	itemSlideNumber.Text = "Total Slides: " + pageCount.ToString();
			//});

		}


		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, e);
			}
		}

		#region Logical page navigation

		// The split page isdesigned so that when the Window does have enough space to show
		// both the list and the dteails, only one pane will be shown at at time.
		//
		// This is all implemented with a single physical page that can represent two logical
		// pages.  The code below achieves this goal without making the user aware of the
		// distinction.

		private const int MinimumWidthForSupportingTwoPanes = 768;

		/// <summary>
		/// Invoked to determine whether the page should act as one logical page or two.
		/// </summary>
		/// <returns>True if the window should show act as one logical page, false
		/// otherwise.</returns>
		private bool UsingLogicalPageNavigation()
		{
			return Window.Current.Bounds.Width < MinimumWidthForSupportingTwoPanes;
		}

		/// <summary>
		/// Invoked with the Window changes size
		/// </summary>
		/// <param name="sender">The current Window</param>
		/// <param name="e">Event data that describes the new size of the Window</param>
		private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
		{
			this.InvalidateVisualState();
		}

		/// <summary>
		/// Invoked when an item within the list is selected.
		/// </summary>
		/// <param name="sender">The GridView displaying the selected item.</param>
		/// <param name="e">Event data that describes how the selection was changed.</param>
		private void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (itemListView.SelectedIndex >= 0)
			{
				itemDetailGrid.Visibility = Visibility.Visible;
				SelectItemPleaseInstruction.Visibility = Visibility.Collapsed;
			}
			else
			{
				itemDetailGrid.Visibility = Visibility.Collapsed;
				SelectItemPleaseInstruction.Visibility = Visibility.Visible;
			}
			if (itemListView.SelectedItems != null && itemListView.SelectedItems.Count > 1)
			{
				if (this.BottomAppBar != null)
					this.BottomAppBar.IsOpen = true;
				SelectItemPleaseInstruction.Text = "상세 정보는 한 개만 선택할 때 볼 수 있습니다.";
			}

			// Invalidate the view state when logical page navigation is in effect, as a change
			// in selection may cause a corresponding change in the current logical page. When
			// an item is selected this has the effect of changing from displaying the item list
			// to showing the selected item's details.  When the selection is cleared this has the
			// opposite effect.
			if (this.UsingLogicalPageNavigation()) this.InvalidateVisualState();

		}

		private bool CanGoBack()
		{
			if (this.UsingLogicalPageNavigation() && this.itemListView.SelectedItem != null)
			{
				return true;
			}
			else
			{
				return this.navigationHelper.CanGoBack();
			}
		}
		private void GoBack()
		{
			if (this.UsingLogicalPageNavigation() && this.itemListView.SelectedItem != null)
			{
				// When logical page navigation is in effect and there's a selected item that
				// item's details are currently displayed.  Clearing the selection will return to
				// the item list.  From the user's point of view this is a logical backward
				// navigation.
				this.itemListView.SelectedItem = null;
			}
			else
			{
				// CreatePage에 대응하기 위해서 처리. CreatePage가 Cached이므로, LoadState가 먹지 않는다.
				_viewModel.SelectedBoard = null;
				this.navigationHelper.GoBack();
			}
		}

		private void InvalidateVisualState()
		{
			var visualState = DetermineVisualState();
			VisualStateManager.GoToState(this, visualState, false);
			this.navigationHelper.GoBackCommand.RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Invoked to determine the name of the visual state that corresponds to an application
		/// view state.
		/// </summary>
		/// <returns>The name of the desired visual state.  This is the same as the name of the
		/// view state except when there is a selected item in portrait and snapped views where
		/// this additional logical page is represented by adding a suffix of _Detail.</returns>
		private string DetermineVisualState()
		{
			if (!UsingLogicalPageNavigation())
				return "PrimaryView";

			// Update the back button's enabled state when the view state changes
			var logicalPageBack = this.UsingLogicalPageNavigation() && this.itemListView.SelectedItem != null;

			return logicalPageBack ? "SinglePane_Detail" : "SinglePane";
		}

		#endregion

		#region NavigationHelper registration

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

		#endregion

		private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(CreatePage), _viewModel.SelectedBoard);
		}

		private void AppBarRemoveButton_Click(object sender, RoutedEventArgs e)
		{
			if (itemListView.SelectedItem != null || itemListView.SelectedItems != null)
			{
				// TODO: 선택 아이템 삭제.	

			}
		}
	}
}
