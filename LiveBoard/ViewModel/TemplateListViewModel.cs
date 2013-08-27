using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Windows.Data.Xml.Dom;
using GalaSoft.MvvmLight;
using LiveBoard.Model;

namespace LiveBoard.ViewModel
{
	public class TemplateListViewModel: ObservableCollection<LbTemplate>
	{
		public TemplateListViewModel()
		{
			if (ViewModelBase.IsInDesignModeStatic)
			{
				// Code runs in Blend --> create design time data.
			}
			else
			{
				readFile();
			}
		}

		private async void readFile()
		{
			// XML 읽기. http://prathapk.net/?p=5
			var storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("PageTemplate");
			var storageFile = await storageFolder.GetFileAsync("TemplateList.xml");
			var xmlDoc = await XmlDocument.LoadFromFileAsync(storageFile);
			var xElement = XElement.Parse(xmlDoc.GetXml());
			foreach (var element in xElement.Elements("Template"))
			{
				this.Add(LbTemplate.FromXml(element));
			}
		}
	}
}
