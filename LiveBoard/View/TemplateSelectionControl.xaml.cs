using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;

namespace LiveBoard.View
{
	public sealed partial class TemplateSelectionControl : UserControl
	{
		public TemplateSelectionControl()
		{
			this.InitializeComponent();
		}

		private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
		{
			Messenger.Default.Send(new GenericMessage<LbMessage>(new LbMessage()
			{
				MessageType = LbMessageType.EVT_PAGE_CREATING,
				Data = ComboBoxTemplate.SelectedItem
			}));
			// 창 닫기
		}
	}
}
