using System;
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Model;

namespace LiveBoard.Common
{
    /// <summary>
    /// <see cref="Messenger"/>를 통해 전송되는 데이터
    /// </summary>
    public class LbMessage
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LbMessage()
        {
            TimeStamp = DateTime.Now;
        }

        /// <summary>
        /// 자동입력
        /// </summary>
        public readonly DateTime TimeStamp;
        /// <summary>
        /// 메시지 타입
        /// </summary>
        public LbMessageType MessageType;
        /// <summary>
        /// 전송 데이터
        /// </summary>
        public object Data;
		/// <summary>
		/// 보드
		/// </summary>
	    public Board Board;
    }
}