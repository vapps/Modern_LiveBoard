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

		private Popup _popup;

	}
}
