﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;
using LiveBoard.PageTemplate.Model;
using LiveBoard.ViewModel;
using Telerik.UI.Xaml.Controls.Input;

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
				else if (message.Content.MessageType == LbMessageType.EVT_PAGE_STARTED)
				{
					// 프리뷰의 페이지가 로딩되었을 때.
				}
			});
		}

		/// <summary>
		/// 템플릿 로딩.
		/// </summary>
		/// <param name="templateCode"><see cref="Windows.UI.Xaml.Controls.IPage"/>내의 View.</param>
		private void loadFrame(string templateCode)
		{
			// 오브젝트 이름에 따라 자동으로 뷰 템플릿 로딩.
			var t = Type.GetType("LiveBoard.PageTemplate.View." + templateCode);
			if (t != null)
				FramePreview.Navigate(t);
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

		private void NumericBoxDuration_OnValueChanged(object sender, EventArgs e)
		{
			var numericBox = sender as RadNumericBox;
			if (numericBox == null)
				return;
			var page = numericBox.DataContext as IPage;
			if (page == null)
				return;

			//page.Duration = new TimeSpan(0, (int)NumericBoxMinute.Value.GetValueOrDefault(page.Duration.Minutes), (int)NumericBoxSecond.Value.GetValueOrDefault(page.Duration.Seconds));
		}

		private void ListViewPages_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			// Duration 바뀌는 것 처리.
			var listView = sender as ListView;
			if (listView == null)
				return;
			var page = listView.SelectedItem as IPage;
			if (page == null)
				return;
			_viewModel.CurrentPage = page;
			loadFrame(_viewModel.CurrentPage.View);
		}

		private void ButtonAddPage_OnClick(object sender, RoutedEventArgs e)
		{
			//var popup = new PopupHelper(new TemplateSelectionControl());
			//popup.ShowAsync();
			//_viewModel.AddPageCmd.Execute(null);
		}


		private void SliderMinute_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			var slider = sender as Slider;
			if (slider == null)
				return;
			var page = slider.DataContext as IPage;
			if (page == null)
				return;
			page.Duration = new TimeSpan(0, (int)SliderMinute.Value, (int)SliderSecond.Value);
		}

		private void SliderSecond_OnValueChanged(object sender, RangeBaseValueChangedEventArgs e)
		{
			var slider = sender as Slider;
			if (slider == null)
				return;
			var page = slider.DataContext as IPage;
			if (page == null)
				return;
			page.Duration = new TimeSpan(0, (int)SliderMinute.Value, (int)SliderSecond.Value);
		}

		private void GridDetails_OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
		{
			if (GridDetails == null)
				return;
			var page = GridDetails.DataContext as IPage;
			if (page == null)
				return;

			// 어쩐지 바인딩이 적용되지 않아서 직접 해줘야 함.
			SliderMinute.Value = page.Duration.Hours * 60 + page.Duration.Minutes;
			SliderSecond.Value = page.Duration.Seconds;
		}

	}
}
