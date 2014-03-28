using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel.Resources;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;
using LiveBoard.PageTemplate.Model;
using LiveBoard.ViewModel;

namespace LiveBoard.View
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class ShowPage : Page
	{
		private NavigationHelper navigationHelper;
		// TODO: 커서 감추기. http://blogs.msdn.com/b/devfish/archive/2012/08/02/customcursors-in-windows-8-csharp-metro-applications.aspx
		readonly ResourceLoader _loader = new ResourceLoader("Resources");

		/// <summary>
		/// NavigationHelper is used on each page to aid in navigation and 
		/// process lifetime management
		/// </summary>
		public NavigationHelper NavigationHelper
		{
			get { return this.navigationHelper; }
		}

		private int _counterForTipShowing = 0;
		private readonly MainViewModel _vm;
		public ShowPage()
		{
			this.InitializeComponent();
			this.navigationHelper = new NavigationHelper(this);
			this.navigationHelper.LoadState += navigationHelper_LoadState;
			this.navigationHelper.SaveState += navigationHelper_SaveState;

			if (_vm == null)
				_vm = DataContext as MainViewModel;

			// 메신저 등록
			Messenger.Default.Register<GenericMessage<LbMessage>>(this, message =>
			{
				// Debug.WriteLine("* ShowPage.xaml.cs Received Message: " + message.Content.MessageType.ToString());
				switch (message.Content.MessageType)
				{
					case LbMessageType.EVT_PAGE_READY:
						loadFrame(_vm.CurrentPage.View);
						break;
					case LbMessageType.EVT_TICK:
						// 팁을 보이고 있다면 판단하여 숨긴다.
						if (GridTipBanner.Visibility == Visibility.Visible && !_vm.IsPreview && _counterForTipShowing > 0)
						{
							--_counterForTipShowing;
							if (_counterForTipShowing <= 0)
							{
								_counterForTipShowing = 0;
								GridTipBanner.Visibility = Visibility.Collapsed;
							}
						}
						break;
				}
			});
		}

		/// <summary>
		/// 템플릿 로딩.
		/// </summary>
		/// <param name="templateCode"><see cref="IPage"/>내의 View.</param>
		private void loadFrame(string templateCode)
		{
			// 오브젝트 이름에 따라 자동으로 뷰 템플릿 로딩.
			var t = Type.GetType("LiveBoard.PageTemplate.View." + templateCode);
			if (t != null)
				FrameRoot.Navigate(t);
			else
			{
				Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
				{
					MessageType = LbMessageType.ERROR,
					Data = LbError.PageTemplateViewTypeNotFound
				}));
			}

		}


		/// <summary>
		/// Populates the page with content passed during navigation. Any saved state is also
		/// provided when recreating a page from a prior session.
		/// </summary>
		/// <param name="sender">
		/// The source of the event; typically <see cref="NavigationHelper"/>
		/// </param>
		/// <param name="e">Event data that provides both the navigation parameter passed to
		/// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
		/// a dictionary of state preserved by this page during an earlier
		/// session. The state will be null the first time a page is visited.</param>
		private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
		{
			if (_vm == null)
			{
				new MessageDialog(_loader.GetString("ErrorShowPlaying")).ShowAsync();
				Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
				{
					MessageType = LbMessageType.ERROR,
					Data = LbError.DataContextIsNull
				}));
				return;
			}

			// Preview 표시 노출.
			GridPreviewBanner.Visibility = _vm.IsPreview ? Visibility.Visible : Visibility.Collapsed;
			//ProgressBarTimer.Visibility = _vm.IsPreview ? Visibility.Visible : Visibility.Collapsed;

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
		/// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
		/// <param name="e">Event data that provides an empty dictionary to be populated with
		/// serializable state.</param>
		private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
		{
		}

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
			// 페이지를 벗어날 때 종료 메시지 전송.
			Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
			{
				MessageType = LbMessageType.EVT_SHOW_FINISHING
			}));
			navigationHelper.OnNavigatedFrom(e);
		}

		#endregion


		private void pageRoot_Tapped(object sender, TappedRoutedEventArgs e)
		{
			showTip();
		}

		private void pageRoot_PointerMoved(object sender, PointerRoutedEventArgs e)
		{
			showTip();
		}

		/// <summary>
		/// 플레이 중에 마우스 움직이거나 탭하면 팁 보이기
		/// </summary>
		private void showTip()
		{
			if (_vm.IsPreview) 
				return;
			GridTipBanner.Visibility = Visibility.Visible;
			_counterForTipShowing = 3;
		}

		private void pageRoot_Loaded(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("ShowPage loaded: {0}x{1}", this.ActualWidth, this.ActualHeight);
		}
	}
}
