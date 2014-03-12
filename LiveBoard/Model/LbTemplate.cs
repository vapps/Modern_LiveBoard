using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using LiveBoard.PageTemplate.Model;

namespace LiveBoard.Model
{
	/// <summary>
	/// 템플릿
	/// </summary>
	public class LbTemplate
	{
		public string Key { get; set; }
		public string DisplayName { get; set; }
		public string Description { get; set; }
		public string TemplateView { get; set; }
		public string TemplateModel { get; set; }

		public List<LbPageData> DataList
		{
			get;
			set;
		}

		public static LbTemplate FromXml(XElement xElement)
		{
			/**
			 * 	<Template Key="SingleStringPage" DisplayName="1-line text" Description="Show single line of text" TemplateView="SimpleText" TemplateModel="SingleStringPage">
					<DataList>
						<Data Key="Text" Name="Text" ValueType="String" DefaultData="" />
					</DataList>
				</Template>
			 * */
			var template = new LbTemplate();
			template.Key = xElement.Attribute("Key").Value;
			template.DisplayName = xElement.Attribute("Name").Value;
			template.Description = xElement.Attribute("Description").Value;
			template.TemplateView = xElement.Attribute("TemplateView").Value;
			template.TemplateModel = xElement.Attribute("TemplateModel").Value;

			// 다국어 처리.
			try
			{
				var loader = new Windows.ApplicationModel.Resources.ResourceLoader("TemplateList");
				var localeName = loader.GetString(String.Format("{0}/{1}", template.Key, "Name"));
				if (!String.IsNullOrEmpty(localeName))
					template.DisplayName = localeName;				
				var localeDescription = loader.GetString(String.Format("{0}/{1}", template.Key, "Description"));
				if (!String.IsNullOrEmpty(localeDescription))
					template.Description = localeDescription;
			}
			catch (Exception)
			{
			}

			if (xElement.Element("DataList") != null && xElement.Element("DataList").HasElements)
				template.DataList = new List<LbPageData>();
			else
				return template;

			foreach (var data in xElement.Element("DataList").Elements("Data"))
			{
				template.DataList.Add(LbPageData.FromXml(template.Key, data));
			}

			Debug.WriteLine(typeof(IEnumerable<string>));
			return template;
		}
	}
}