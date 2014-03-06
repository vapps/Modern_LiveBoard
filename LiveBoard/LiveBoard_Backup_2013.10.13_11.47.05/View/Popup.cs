using System;
using SocialEbola.Lib.PopupHelpers;

namespace LiveBoard.View
{
	/// <summary>
	/// 팝업 클래스.
	/// </summary>
	public class Popup : PopupHelper<TemplateSelectionControl>
	{
		private readonly PopupSettings _settings = new PopupSettings(TimeSpan.FromMilliseconds(350), 0.7, 0.5, PopupAnimation.OverlayFade, false);
		/// <summary>
		/// 팝업 셋팅을 기본애니메이션(FlipControl)에서 변경해줌.
		/// <para>이렇게 안하면 터치시 앱이 죽는다. <![CDATA[http://socialeboladev.wordpress.com/2012/10/14/turn-any-usercontrol-into-a-pleasing-dialogflyout-in-windows-8/]]>참고</para>
		/// </summary>
		public override PopupSettings Settings
		{
			get { return _settings; }
		}

	}
}