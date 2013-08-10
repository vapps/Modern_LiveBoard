using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace LiveBoard.Common
{
    /// <summary>
    /// <see cref="Messenger"/> 를 통해 전송되는 이벤트 규약
    /// </summary>
    public enum LbMessageType
    {
		EVT_ON_DATA_READY,
		EVT_PAGE_STARTED,
		EVT_PAGE_FINISHED,
	    EVT_PAGE_FINISHING,
        EVT_TICK,
        ERROR,
        EVT_SHOW_END,
	    EVT_SHOW_STARTING,
        EVT_SHOW_STARTED,
	    EVT_SHOW_FINISHING,
	    EVT_SHOW_FINISHED
    }

	/// <summary>
	/// 에러 유형
	/// </summary>
	public enum LbError
	{
		UnSnappedToSave,
		DataContextIsNull,
		IsPlayingTrue
	}

}
