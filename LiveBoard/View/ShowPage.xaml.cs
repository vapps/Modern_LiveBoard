using System;
using System.Collections.Generic;
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
            // 처음 뜨는 것을 알림.
            Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
            {
                MessageType = LbMessageType.EVT_SHOWSTARTED            }));

            if (_vm == null)
                _vm = DataContext as MainViewModel;
            if (_vm == null)
            {
                Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
                {
                    MessageType = LbMessageType.ERROR,
                    Data = "datacontext null"
                }));                
                return;
            }

            // Preview 표시 노출.
            GridPreviewBanner.Visibility = _vm.IsPreview ? Visibility.Visible : Visibility.Collapsed;

            // first load
            if (pageState == null)
            {
                _vm.PlayCmd.Execute(_vm.ActiveBoard);

                // TODO: 시나리오에 따라 바꿔줌.
                FrameRoot.Navigate(typeof (BlankPage_SingleText));

            }
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
