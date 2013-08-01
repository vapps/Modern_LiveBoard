using System;
using System.Collections.Generic;
using LiveBoard.Data;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231
using LiveBoard.View;

namespace LiveBoard
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class MainHubPage : LiveBoard.Common.LayoutAwarePage
    {
        public MainHubPage()
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
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var data = new SampleDataSource();
            this.DefaultViewModel["Groups"] = data.SampleItems;
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        private async void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemTitle = ((SampleDataCommon)e.ClickedItem);

            //MessageDialog dialog = new MessageDialog("You clicked on " + itemTitle, "Selected Item");
            //await dialog.ShowAsync();

            // VM의 SelectedQuestion을 통해 선택된 질문 정보가 담기므로, o.Content는 안해도 되지만,
            // 보험차원에서 전송.
            Frame.Navigate(typeof(CreatePage));
        }
    }
}
