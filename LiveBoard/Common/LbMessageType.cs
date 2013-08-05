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
        EVT_SHOWSTARTED,
        EVT_SHOWEND,
        EVT_TICK,
        ERROR
    }

}
