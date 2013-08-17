using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;
using LiveBoard.PageTemplate.Model;
using LiveBoard.ViewModel;

namespace LiveBoard.View
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class ShowPage : LiveBoard.Common.LayoutAwarePage
	{
		// TODO: 커서 감추기. http://blogs.msdn.com/b/devfish/archive/2012/08/02/customcursors-in-windows-8-csharp-metro-applications.aspx

		private int _counterForTipShowing = 0;
		private readonly MainViewModel _vm;
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
					case LbMessageType.EVT_PAGE_READY:
						loadFrame(_vm.CurrentPage.TemplateCode);
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
		/// <param name="templateCode"><see cref="IPage"/>내의 TemplateCode.</param>
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
					Data = LbError.UnSnappedToSave
				}));
				return;
			}

			// Preview 표시 노출.
			GridPreviewBanner.Visibility = _vm.IsPreview ? Visibility.Visible : Visibility.Collapsed;
			ProgressBarTimer.Visibility = _vm.IsPreview ? Visibility.Visible : Visibility.Collapsed;

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
	}
}
