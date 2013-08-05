using System;
using System.Diagnostics;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;
using LiveBoard.Model;
using LiveBoard.Model.Page;

namespace LiveBoard.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private Board _activeBoard = new Board();
        private bool _isPreview;
        private bool _isPlaying;
        private DateTime _startTime;
        private IPage _currentPage;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }
        #region ICommand
        public ICommand SaveCmd { get { return new RelayCommand(Save); } }
        public ICommand AddPageCmd { get { return new RelayCommand(AddPage); } }
        public ICommand DeletePageCmd { get { return new RelayCommand<IPage>(DeletePage); } }
        public ICommand PlayCmd { get { return new RelayCommand<Board>(Play); } }

        #endregion ICommand

        /// <summary>
        /// 불러오기.
        /// </summary>
        /// <param name="file"></param>
        public void Load(StorageFile file)
        {
            if (ActiveBoard == null)
                ActiveBoard = new Board();

        }

        /// <summary>
        /// 재생하기
        /// </summary>
        /// <param name="board"></param>
        public void Play(Board board)
        {
            if (!IsPlaying)
                IsPlaying = true;
            else
            {
                Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
                {
                    MessageType = LbMessageType.ERROR, Data = "IsPlaying is true"
                }));
                return;
            }

            StartTime = DateTime.Now;

            var timer = new DispatcherTimer();
            timer.Tick += PlayTimerEventHandler;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void PlayTimerEventHandler(object sender, object e)
        {
            Debug.WriteLine("tick at {0} and Elapsed {1}", DateTime.Now.ToString("u"), (DateTime.Now - StartTime).ToString("g"));

            CurrentPage = ActiveBoard.Pages[0];
            Messenger.Default.Send(new GenericMessage<LbMessage>(this, new LbMessage()
            {
                MessageType = LbMessageType.EVT_TICK
            }));
        }

        /// <summary>
        /// 저장하기.
        /// </summary>
        public void Save()
        {
            ActiveBoard.SaveAsync();
        }

        /// <summary>
        /// 페이지 추가
        /// </summary>
        private void AddPage()
        {
            var page = new SingleTextPage
            {
                Title = "타이틀 " + DateTime.Now.Ticks.ToString(),
                Duration = TimeSpan.FromSeconds(5.0d),
                IsVisible = true,
                Guid = new Guid(),
                TemplateCode = ""
            };
            ActiveBoard.Pages.Add(page);
        }

        /// <summary>
        /// 페이지 삭제
        /// </summary>
        /// <param name="page"></param>
        private void DeletePage(IPage page)
        {
            // TODO: 여러개 삭제하기.
            ActiveBoard.Pages.Remove(page);
        }

        #region Properties

        public IPage CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value; 
                RaisePropertyChanged("CurrentPage");
            }
        }

        /// <summary>
        /// 현재 보드.
        /// </summary>
        public Board ActiveBoard
        {
            get { return _activeBoard; }
            set
            {
                _activeBoard = value; 
                RaisePropertyChanged("ActiveBoard");
            }
        }

        /// <summary>
        /// 미리보기 모드
        /// </summary>
        public bool IsPreview
        {
            get { return _isPreview; }
            set
            {
                _isPreview = value;
                RaisePropertyChanged("IsPreview");
            }
        }

        /// <summary>
        /// 쇼 시작 시간.
        /// </summary>
        public DateTime StartTime
        {
            get { return _startTime; }
            set
            {
                _startTime = value; 
                RaisePropertyChanged("StartTime");
            }
        }

        /// <summary>
        /// 재생 중
        /// </summary>
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                RaisePropertyChanged("IsPlaying");
            }
        }

        #endregion Properties

    }
}