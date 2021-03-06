﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;
using LiveBoard.PageTemplate.Model;
using LiveBoard.ViewModel;
using WinRTXamlToolkit.Controls.Extensions;

namespace LiveBoard.View
{
	/// <summary>
	/// A basic page that provides characteristics common to most applications.
	/// </summary>
	public sealed partial class CreatePage : LiveBoard.Common.LayoutAwarePage
	{
		private MainViewModel _viewModel;
		readonly ResourceLoader _loader = new ResourceLoader("Resources");

		public CreatePage()
		{
			this.InitializeComponent();

			var model = DataContext as MainViewModel;
			if (model != null)
				_viewModel = model;

			//var viewSource = new CollectionViewSource { Source = _viewModel.ActiveBoard.Board.Pages };
			//ListViewPages.ItemsSource = viewSource.View;

			// 메신저 연결.
			Messenger.Default.Register<GenericMessage<LbMessage>>(this, message =>
			{
				var frame = (Frame)Window.Current.Content;
				if (!(frame.Content is CreatePage))
					return;
				switch (message.Content.MessageType)
				{
					case LbMessageType.EVT_SHOW_STARTING:
						{
							Debug.WriteLine("* CreatePage Received Message: " + message.Content.MessageType.ToString());
							if (!(((Frame)Window.Current.Content).Content is ShowPage))
							{
								this.Frame.Navigate(typeof(ShowPage), message.Content.Data);
							}
						}
						break;
					case LbMessageType.EVT_PAGE_CREATING:
						if (BorderTemplateSelection.Visibility == Visibility.Visible)
							ToggleButtonAddPage.IsChecked = false;
						break;
					case LbMessageType.EVT_PAGE_STARTED:
						if (!PreviewLock)
						{
							lockPreview();
						}
						else
						{
							// TODO: 사용자에게 preview중인걸 알리기.
						}
						break;
					case LbMessageType.ERROR:
						{
							if (message.Content.Data is LbError && (LbError)message.Content.Data == LbError.NothingToPlay)
							{
								new MessageDialog(_loader.GetString("ErrorNothingToPlay")).ShowAsync();
							}
						}
						break;
				}

			});
		}

		/// <summary>
		/// 템플릿 로딩.
		/// </summary>
		/// <param name="templateCode"><see cref="Windows.UI.Xaml.Controls.IPage"/>내의 View.</param>
		private void loadFrame(string templateCode = null)
		{
			if (String.IsNullOrEmpty(templateCode))
			{
				// 비워주기.
				FramePreview.Content = null;
				return;
			}

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
		protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
		{
			if (_viewModel == null)
				_viewModel = DataContext as MainViewModel;

			if (_viewModel == null)
				throw new InvalidOperationException("CreatePage.LoadState viewModel is null");
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

			// 미리보기 처리.
			loadFrame(_viewModel.CurrentPage.View);
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

		private void ButtonCloseTemplateSelection_OnClick(object sender, RoutedEventArgs e)
		{
			if (ToggleButtonAddPage.IsChecked.GetValueOrDefault(false))
			{
				ToggleButtonAddPage.IsChecked = false;
			}
		}

		private void pageRoot_Loaded(object sender, RoutedEventArgs e)
		{
			if (_viewModel.SelectedBoard != null)
				_viewModel.ActiveBoard = _viewModel.SelectedBoard;

			// 리스트 바인딩 직접 해줘야 리프래시가 반영됨.
			var viewSource = new CollectionViewSource { Source = _viewModel.ActiveBoard.Board.Pages };
			ListViewPages.ItemsSource = viewSource.View;

			// 아무 페이지도 없을 때 가이드 출력.
			if (_viewModel.ActiveBoard.Board.Pages == null || _viewModel.ActiveBoard.Board.Pages.Count == 0)
				PressPlusButtonInstruction.Visibility = Visibility.Visible;
			else
				PressPlusButtonInstruction.Visibility = Visibility.Collapsed;

			// 선택 페이지에 대한 프리뷰 화면 비워주기.
			if (ListViewPages.SelectedIndex < 0)
			{
				loadFrame(null);
			}
		}

		private void PreviewBorder_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			var ratio = this.ActualWidth / this.ActualHeight;
			FramePreview.Height = FramePreview.ActualWidth / ratio;
		}

		/// <summary>
		/// 프리뷰를 화면비율에 맞게 리사이즈.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FramePreview_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			Debug.WriteLine("navigated");
			PreviewLock = false;

			var ratio = Window.Current.Bounds.Width / FramePreview.ActualWidth;

			var page = (FramePreview.Content as Page);
			if (page != null)
			{
				var myTransformGroup = new TransformGroup();
				myTransformGroup.Children.Add(new ScaleTransform
				{
					ScaleY = 1 / ratio,
					ScaleX = 1 / ratio
				});

				recursiveResize(page.Content, myTransformGroup, ratio);
				page.Content.InvalidateMeasure();
				//var myScaleTransform = new ScaleTransform
				//{
				//	ScaleY = 1 / ratio,
				//	ScaleX = 1 / ratio
				//};
				//var myTransformGroup = new TransformGroup();
				//myTransformGroup.Children.Add(myScaleTransform);
				//var count = page.Content.GetChildren().Count(c => c is UIElement);
				//Debug.WriteLine(count);
				//foreach (var o in page.Content.GetChildren().Where(c => c is UIElement))
				//{
				//	if (o is Panel)
				//	{
				//		var panel = (Panel)o;
				//		(panel).Margin = new Thickness(
				//			panel.Margin.Left / ratio,
				//			panel.Margin.Top / ratio,
				//			panel.Margin.Right / ratio,
				//			panel.Margin.Bottom / ratio);
				//	}
				//	else
				//	{
				//		var element = (UIElement)o;
				//		element.RenderTransformOrigin = new Point(0.5, 0.5);
				//		element.RenderTransform = myTransformGroup;
				//	}
				//}
			}
		}

		private static void recursiveResize(UIElement uiElement, TransformGroup transformGroup, double ratio)
		{

			foreach (var o in uiElement.GetChildren().Where(c => c is UIElement))
			{
				if (o is Panel)
				{
					var panel = (Panel)o;
					(panel).Margin = new Thickness(
						panel.Margin.Left / ratio,
						panel.Margin.Top / ratio,
						panel.Margin.Right / ratio,
						panel.Margin.Bottom / ratio);

					// recursive call.
					recursiveResize((UIElement)o, transformGroup, ratio);
				}
				else if (o is FrameworkElement)
				{
					var element = (FrameworkElement)o;
					element.Margin = new Thickness(
						element.Margin.Left / ratio,
						element.Margin.Top / ratio,
						element.Margin.Right / ratio,
						element.Margin.Bottom / ratio);
					element.RenderTransformOrigin = new Point(0.5, 0.5);
					element.RenderTransform = transformGroup;
				}
			}
		}

		/// <summary>
		/// 프리뷰 잠금.
		/// </summary>
		private void lockPreview()
		{
			PreviewLock = true;
		}

		public bool PreviewLock { get; set; }

		private void ToggleButtonAddPage_OnClick(object sender, RoutedEventArgs e)
		{
			if (PressPlusButtonInstruction.Visibility == Visibility.Visible)
				PressPlusButtonInstruction.Visibility = Visibility.Collapsed;
		}
	}
}
