using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
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

				if (message.Content.MessageType == LbMessageType.EVT_SHOW_STARTING)
				{
					Debug.WriteLine("* CreatePage Received Message: " + message.Content.MessageType.ToString());
					var frame = (Frame)Window.Current.Content;
					if (!(frame.Content is ShowPage))
					{
						if (!PreviewLock)
						{
							lockPreview();
							this.Frame.Navigate(typeof(ShowPage), message.Content.Data);
						}
						else
						{
							// TODO: 사용자에게 preview중인걸 알리기.
						}
					}
				}
				else if (message.Content.MessageType == LbMessageType.EVT_PAGE_CREATING)
				{
					// 템플릿에서 하나의 페이지를 추가할 때: 템플릿 선택 창을 닫는다.
					if (BorderTemplateSelection.Visibility == Visibility.Visible)
						ToggleButtonAddPage.IsChecked = false;
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

		private void FramePreview_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			Debug.WriteLine("navigated");
			PreviewLock = false;

			var ratio = Window.Current.Bounds.Width / FramePreview.ActualWidth;

			var page = (FramePreview.Content as Page);
			if (page != null)
			{
				//page.Width = FramePreview.ActualWidth;
				//page.Height = FramePreview.ActualHeight;
				var myScaleTransform = new ScaleTransform
				{
					ScaleY = 1 / ratio,
					ScaleX = 1 / ratio
				};
				var myTransformGroup = new TransformGroup();
				myTransformGroup.Children.Add(myScaleTransform);
				//page.RenderTransform = myTransformGroup;
				foreach (var o in page.Content.GetChildren().Where(c => c is UIElement))
				{
					var element = (UIElement) o;
					element.RenderTransformOrigin = new Point(0.5, 0.5);
					element.RenderTransform = myTransformGroup;
					// do something with tb here
					//Debug.WriteLine(tb);
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
	}
}
