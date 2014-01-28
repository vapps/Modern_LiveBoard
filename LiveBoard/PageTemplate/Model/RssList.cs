using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.Web.Syndication;
using LiveBoard.Model;

namespace LiveBoard.PageTemplate.Model
{
	public class RssList : SimpleListPage
	{
		/// <summary>
		/// RSS를 불러오는 명령어.
		/// </summary>
		/// <returns></returns>
		public override async Task<bool> PrepareToLoadAsync()
		{
			if (Data == null)
				return false;

			var feedTitles = new ObservableCollection<string>();
			foreach (var templateData in Data)
			{
				if (templateData.Key.Equals("RSS"))
				{
					var url = templateData.Data as string;
					var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;

					// Load RSS feed asyncronously.
					await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
					{
						var feeds = await GetFeeds(url);
						foreach (var item in feeds.Items)
							feedTitles.Add(item.Title);
					});
				}

				if (templateData.Key.Equals("Feeds"))
				{
					templateData.Data = feedTitles;
				}
			}
			return true;
		}


		// ref: Building a Windows 8 RSS Reader
		// http://visualstudiomagazine.com/articles/2012/01/04/building-a-windows-8-rss-reader.aspx

		/// <summary>
		/// RSS 주소에서 피드 가져오기.
		/// </summary>
		/// <param name="url"></param>
		/// <param name="maxItems"></param>
		/// <returns></returns>
		public async Task<RSSFeed> GetFeeds(string url, int maxItems = 10)
		{
			var feeds = new RSSFeed();
			var client = new SyndicationClient();
			var feedUri = new Uri(url);
			SyndicationFeed feed = await client.RetrieveFeedAsync(feedUri);
			var topFeeds = feed.Items.OrderByDescending(x =>
				x.PublishedDate).Take(maxItems).ToList();
			feeds.Title = feed.Title.Text;
			foreach (var item in topFeeds)
			{
				var feedItem = new RSSItem { Title = item.Title.Text, PublishedOn = item.PublishedDate.DateTime };
				var authors = from a in item.Authors
							  select a.Name;
				feedItem.Author =
					String.Join(",", authors);
				feedItem.Content = item.Content !=
								   null ? item.Content.Text : String.Empty;
				feedItem.Description = item.Summary !=
									   null ? item.Summary.Text : String.Empty;
				var links = from l in item.Links
							select new RSSLink(l.Title, l.Uri);
				feedItem.Links = links.ToList();
				feeds.Items.Add(feedItem);
			}
			return feeds;
		}

		public class RSSFeed
		{
			private ObservableCollection<RSSItem> _items = new ObservableCollection<RSSItem>();
			public string Title { get; set; }
			public ObservableCollection<RSSItem> Items
			{
				get
				{
					return _items;
				}
			}
		}

		public class RSSLink
		{
			public string Title { get; set; }
			public Uri Link { get; set; }

			public RSSLink(string title, Uri link)
			{
				Title = !
					String.IsNullOrWhiteSpace(title)
					? title
					: link.AbsoluteUri;
				Link = link;
			}
		}

		public class RSSItem
		{
			public string Title { get; set; }
			public string Author { get; set; }
			public string Content { get; set; }
			public string Description { get; set; }
			public DateTime PublishedOn { get; set; }
			public List<RSSLink> Links { get; set; }
		}
	}
}