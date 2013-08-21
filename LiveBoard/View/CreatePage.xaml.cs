using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using Windows.UI.Xaml.Data;
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;
using LiveBoard.Model;
using LiveBoard.ViewModel;

namespace LiveBoard.View
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class CreatePage : LiveBoard.Common.LayoutAwarePage
	{
		private MainViewModel _viewModel;
		public CreatePage()
		{
			this.InitializeComponent();

			var model = DataContext as MainViewModel;
			if (model != null)
				_viewModel = model;

			var viewSource = new CollectionViewSource { Source = _viewModel.ActiveBoard.Board.Pages };
			ListViewPages.ItemsSource = viewSource.View;

			// 메신저 연결.
			Messenger.Default.Register<GenericMessage<LbMessage>>(this, message =>
			{

				if (message.Content.MessageType == LbMessageType.EVT_SHOW_STARTING)
				{
					Debug.WriteLine("* CreatePage Received Message: " + message.Content.MessageType.ToString());
					this.Frame.Navigate(typeof(ShowPage), message.Content.Data);
				}
			});
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

			//if (_viewModel != null && navigationParameter == null)
			//	_viewModel.ActiveBoard.Clear();
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

	}
}
