using System;
using System.Threading.Tasks;

namespace LiveBoard.PageTemplate.Model
{
	/// <summary>
	/// 페이지 하나의 공용 인터페이스
	/// </summary>
	public interface IPage
	{
		/// <summary>
		/// GUID
		/// </summary>
		string Guid { get; set; }
		/// <summary>
		/// 타이틀
		/// </summary>
		string Title { get; set; }
		/// <summary>
		/// 뷰템플릿 키
		/// </summary>
		string View { get; set; }
		/// <summary>
		/// 템플릿에 대한 옵션. 세미콜론(;)으로 구분함.
		/// </summary>
		string ViewOption { get; set; }
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

		object Data { get; set; }

		/// <summary>
		/// CurrentPage로 로딩될 때 호출됨. 데이터를 미리 준비시킬 수 있다.
		/// </summary>
		/// <returns></returns>
		Task<bool> PrepareToLoadAsync();
	}
}
