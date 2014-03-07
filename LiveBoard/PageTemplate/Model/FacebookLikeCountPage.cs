using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
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
			if (Data == null)
				return false;

			int count = 0;
			foreach (var templateData in Data)
			{
				if (templateData.Key == "Page")
				{
					var pageName = templateData.Data as string;
					var defaultPageName = templateData.DefaultData as string;
					var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
					var httpClient = new HttpClient();

					// http://graph.facebook.com/mabllabs?fields=likes
					var address = String.Format("http://graph.facebook.com/{0}?fields=likes", !String.IsNullOrWhiteSpace(pageName) ? pageName : defaultPageName);
					var json = JsonObject.Parse(await httpClient.GetStringAsync(address));
					count = (int)json.GetNamedNumber("likes", 0d);
				}

				if (templateData.Key == "Count")
				{
					templateData.Data = count;
				}
			}
			return true;
		}

	}
}