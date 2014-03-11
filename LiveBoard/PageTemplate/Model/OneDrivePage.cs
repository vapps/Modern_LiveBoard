using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using Microsoft.Live;

namespace LiveBoard.PageTemplate.Model
{
	public class OneDrivePage : ObservableObject, IPage
	{
		public OneDrivePage()
		{
			Guid = new Guid().ToString();

			if (App.Session == null)
			{
				InitAuth();
			}
		}


		private async void InitAuth()
		{
			if (!Windows.ApplicationModel.DesignMode.DesignModeEnabled)
			{
				var authClient = new LiveAuthClient();
				LiveLoginResult authResult =
					await authClient.LoginAsync(new List<string>() { "wl.signin", "wl.basic", "wl.skydrive" });
				if (authResult.Status == LiveConnectSessionStatus.Connected)
				{
					// An app-level property for the session.
					App.Session = authResult.Session;
				}
			}
		}


		public string Guid { get; set; }

		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				RaisePropertyChanged("Title");
			}
		}

		public string View { get; set; }
		public string ViewOption { get; set; }

		public TimeSpan Duration
		{
			get { return _duration; }
			set
			{
				_duration = value;
				RaisePropertyChanged("Duration");
			}
		}

		public string Description
		{
			get { return _description; }
			set
			{
				_description = value;
				RaisePropertyChanged("Description");
			}
		}

		public bool IsVisible { get; set; }
		public string TemplateKey { get; set; }

		public virtual async Task<bool> PrepareToLoadAsync()
		{
			// do nothing.
			return true;
		}

		public XElement ToXml()
		{
			var xElement = new XElement("Page",
				new XAttribute("Title", Title ?? ""),
				new XAttribute("IsVisible", IsVisible),
				new XAttribute("Description", Description ?? ""),
				new XAttribute("Guid", Guid ?? new Guid().ToString()),
				new XAttribute("Duration", Duration.TotalMilliseconds),
				new XAttribute("TemplateKey", TemplateKey ?? ""),
				new XAttribute("View", View ?? ""),
				new XAttribute("ViewOption", ViewOption ?? ""),
				new XElement("DataList", Data.Select(d => d.ToXml())));
			return xElement;
		}

		/// <summary>
		/// Specific data.
		/// </summary>
		public IEnumerable<LbPageData> Data
		{
			get { return _data; }
			set
			{
				_data = value;
				RaisePropertyChanged("Data");
			}
		}

		public override string ToString()
		{
			return String.Format("{0} ({1})", Title, View);
		}

		private IEnumerable<LbPageData> _data;
		private string _title;
		private TimeSpan _duration;
		private string _description;
	}
}

