using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;
using LiveBoard.ViewModel;

namespace LiveBoard.View
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class StartPage : LiveBoard.Common.LayoutAwarePage
	{
		private MainViewModel _viewModel;
		public StartPage()
		{
			this.InitializeComponent();

			var model = DataContext as MainViewModel;
			if (model != null)
				_viewModel = model;
		}

		/// <summary>
		/// Populates the page with content passed during navigation.  Any saved state is also
		/// provided when recreating a page from a prior session.
		/// </summary>
		/// <param name="navigationParameter">The parameter value passed to
		/// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
		/// </param>
		/// <param name="pageState">A dictionary of state preserved by this page during an earlier
		/// session.  This will be null the first time a page is visited.</param>
		protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
		{
			if (_viewModel == null)
				_viewModel = DataContext as MainViewModel;
		}

		/// <summary>
		/// Preserves state associated with this page in case the application is suspended or the
		/// page is discarded from the navigation cache.  Values must conform to the serialization
		/// requirements of <see cref="SuspensionManager.SessionState"/>.
		/// </summary>
		/// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
		protected override void SaveState(Dictionary<String, Object> pageState)
		{
		}

		private void ButtonCreate_OnClick(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(CreatePage));
		}

		private void ButtonOpen_OnClick(object sender, RoutedEventArgs e)
		{
			Load();
		}

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

			if (_viewModel.ActiveBoard == null)
				_viewModel.ActiveBoard = new BoardViewModel();

			await _viewModel.ActiveBoard.LoadAsync(file, _viewModel.Templates);
			// 최근 문서로 저장.
			// http://msdn.microsoft.com/en-us/library/windows/apps/hh972344.aspx
			string mruToken = StorageApplicationPermissions.MostRecentlyUsedList.Add(file, file.DisplayName);
			saveOpenedFile(mruToken);

			Frame.Navigate(typeof(CreatePage), _viewModel.ActiveBoard);
		}

		/// <summary>
		/// 최근 문서 어레이에 저장.
		/// </summary>
		/// <param name="filename"></param>
		private void saveOpenedFile(string filename)
		{
			if (_viewModel == null)
				throw new ArgumentNullException("filename");
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
				await new Windows.UI.Popups.MessageDialog("No recent files. Please creat a new Board.").ShowAsync();
				return;
			}

			try
			{
				String mruFirstToken = StorageApplicationPermissions.MostRecentlyUsedList.Entries.First().Token;
				StorageFile storageFile = await StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(mruFirstToken);
				await _viewModel.ActiveBoard.LoadAsync(storageFile, _viewModel.Templates);
			}
			catch (Exception exception)
			{
				new Windows.UI.Popups.MessageDialog(exception.Message + "File loading error. File is corrupted or wrongly saved").ShowAsync();
				return;
			}

			this.Frame.Navigate(typeof(ShowPage), _viewModel.ActiveBoard);
		}

		private void ButtonEditRecent_OnClick(object sender, RoutedEventArgs e)
		{
			Frame.Navigate(typeof(RecentOpenedPage));
		}
	}
}
