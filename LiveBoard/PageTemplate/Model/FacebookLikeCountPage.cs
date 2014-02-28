using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Data.Json;
using Windows.UI.Core;

namespace LiveBoard.PageTemplate.Model
{
	public class FacebookLikeCountPage : SingleStringPage
	{
		/// <summary>
		/// Facebook Like.
		/// </summary>
		/// <returns></returns>
		public override async Task<bool> PrepareToLoadAsync()
		{
			var list = Data as IEnumerable<LbPageData>;
			if (list == null)
				return false;

			var feedTitles = new ObservableCollection<string>();
			foreach (var templateData in list)
			{
				if (templateData.Key == "PageName")
				{
					var url = templateData.Data as string;
					var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

					// Load Facebook like json asyncronously.
					await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
					{
						var json = new JsonObject();
						// http://graph.facebook.com/mabllabs?fields=likes
					});
				}

				if (templateData.Key == "Feeds")
				{
					templateData.Data = feedTitles;
				}
			}
			return true;
		}

	}
}