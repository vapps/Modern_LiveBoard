using GalaSoft.MvvmLight.Messaging;

namespace LiveBoard.Common
{
    /// <summary>
    /// <see cref="Messenger"/> 를 통해 전송되는 이벤트 규약
    /// </summary>
    public enum LbMessageType
    {
		/// <summary>
		/// MainViewModel에서 CurrentPage에 대한 데이터가 로딩 완료했을 때.
		/// </summary>
	    EVT_PAGE_READY,
		EVT_PAGE_STARTED,
		EVT_PAGE_FINISHED,
	    EVT_PAGE_FINISHING,
		/// <summary>
		/// 슬라이드가 시작된 후 매초마다 발생.
		/// </summary>
        EVT_TICK,
		/// <summary>
		/// 에러 브로드캐스트.
		/// </summary>
        ERROR,
        EVT_SHOW_END,
	    EVT_SHOW_STARTING,
        EVT_SHOW_STARTED,
	    EVT_SHOW_FINISHING,
	    EVT_SHOW_FINISHED,
		/// <summary>
		/// 페이지 생성 명령
		/// </summary>
	    EVT_PAGE_CREATING
    }
}
