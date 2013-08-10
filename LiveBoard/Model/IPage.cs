using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveBoard.Model
{
    /// <summary>
    /// 페이지 하나의 공용 인터페이스
    /// </summary>
    public interface IPage
    {
        /// <summary>
        /// GUID
        /// </summary>
        Guid Guid { get; set; }
        /// <summary>
        /// 타이틀
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// 템플릿 코드
        /// </summary>
        string TemplateCode { get; set; }
        /// <summary>
        /// 템플릿에 대한 옵션. 세미콜론(;)으로 구분함.
        /// </summary>
        string TemplateOption { get; set; }
        /// <summary>
        /// 재생 시간
        /// </summary>
        TimeSpan Duration { get; set; }
        /// <summary>
        /// 추가 설명
        /// </summary>
        string Description { get; set; }
        /// <summary>
        /// 보이기
        /// </summary>
        bool IsVisible { get; set; }


    }
}
