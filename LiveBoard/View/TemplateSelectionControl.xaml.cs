using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using Windows.UI.Xaml.Controls.Primitives;
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;
using SocialEbola.Lib.PopupHelpers;

namespace LiveBoard.View
{
	public sealed partial class TemplateSelectionControl : UserControl, IPopupControl
	{
		public TemplateSelectionControl()
		{
			this.InitializeComponent();
		}

		public void SetParent(PopupHelper parent)
		{
			_popup = (Popup)parent;
		}

		public void Closed(CloseAction closeAction)
		{
		}

		public void Opened()
		{
		}

		private void ButtonOk_OnClick(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send(new GenericMessage<LbMessage>(new LbMessage()
			{
				MessageType = LbMessageType.EVT_PAGE_CREATING,
				Data = ComboBoxTemplate.SelectedItem
			}));
			var dummy = _popup.CloseAsync();
		}

		/// <summary>
		/// 팝업 클래스.
		/// </summary>
		public class Popup : PopupHelper<TemplateSelectionControl>
		{
			private readonly PopupSettings _settings = new PopupSettings(TimeSpan.FromMilliseconds(350), 0.7, 0.5, PopupAnimation.OverlayFade, false);
			/// <summary>
			/// 팝업 셋팅을 기본애니메이션(FlipControl)에서 변경해줌.
			/// <para>이렇게 안하면 터치시 앱이 죽는다. <![CDATA[http://socialeboladev.wordpress.com/2012/10/14/turn-any-usercontrol-into-a-pleasing-dialogflyout-in-windows-8/]]>참고</para>
			/// </summary>
			public override PopupSettings Settings
			{
				get { return _settings; }
			}

		}

		private Popup _popup;
	}
}
