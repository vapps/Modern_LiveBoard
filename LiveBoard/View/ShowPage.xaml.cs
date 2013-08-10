using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
using LiveBoard.Pages;
using LiveBoard.ViewModel;

namespace LiveBoard.View
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class ShowPage : LiveBoard.Common.LayoutAwarePage
	{
		private MainViewModel _vm;
		public ShowPage()
		{
			this.InitializeComponent();

			if (_vm == null)
				_vm = DataContext as MainViewModel;

			// 메신저 등록
			Messenger.Default.Register<GenericMessage<LbMessage>>(this, message =>
			{
				// Debug.WriteLine("* ShowPage.xaml.cs Received Message: " + message.Content.MessageType.ToString());

				switch (message.Content.MessageType)
				{
					case LbMessageType.EVT_SHOW_STARTED:
						if (_vm.CurrentPage.TemplateCode == "BlankPage_SingleText")
						{
							FrameRoot.Navigate(typeof(BlankPage_SingleText));
						}
						break;
					case LbMessageType.EVT_PAGE_FINISHING:
						if (_vm.CurrentPage.TemplateCode == "BlankPage_SingleText")
						{
							FrameRoot.Navigate(typeof(BlankPage_SingleText));
						}
						break;
				}
			});
		}

		/// <summary>
		/// back을 할 때 쇼 종료 명령도 브로드캐스트.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void GoBack(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
			{
				MessageType = LbMessageType.EVT_SHOW_FINISHING
			}));
			base.GoBack(sender, e);
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
			if (_vm == null)
			{
				Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
				{
					MessageType = LbMessageType.ERROR,
					Data = "ShowPage: datacontext null"
				}));
				return;
			}

			// Preview 표시 노출.
			GridPreviewBanner.Visibility = _vm.IsPreview ? Visibility.Visible : Visibility.Collapsed;

			Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
			{
				MessageType = LbMessageType.EVT_SHOW_STARTED,
			}));
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
