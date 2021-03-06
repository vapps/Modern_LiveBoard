﻿using System;
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
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;

namespace LiveBoard.PageTemplate.View
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class StaticWebView : Page
	{
		public StaticWebView()
		{
			this.InitializeComponent();
			Messenger.Default.Send(new GenericMessage<LbMessage>(new LbMessage()
			{
				MessageType = LbMessageType.EVT_PAGE_STARTED
			}));
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.  The Parameter
		/// property is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
		}

		private void MainWebView_OnLoadCompleted(object sender, NavigationEventArgs e)
		{
			var brush = new WebViewBrush();
			brush.SetSource(MainWebView);
			brush.Redraw();
			RectWebViewBrush.Fill = brush;
			RectWebViewBrush.Visibility = Visibility.Visible;
			MainWebView.Visibility = Visibility.Collapsed;
		}
	}
}
