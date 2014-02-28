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
		private string _filename = "TemplateList.xml";
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

		/// <summary>
		/// 템플릿으로부터 생성
		/// </summary>
		/// <param name="filename">템플릿 XML 파일 경로</param>
		public TemplateListViewModel(string filename)
		{
			readFile(filename);
		}

		private async void readFile(string filename = null)
		{
			if (!String.IsNullOrEmpty(filename))
				_filename = filename;

			// XML 읽기. http://prathapk.net/?p=5
			var storageFolder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("PageTemplate");
			var storageFile = await storageFolder.GetFileAsync(filename ?? _filename);
			var xmlDoc = await XmlDocument.LoadFromFileAsync(storageFile);
			var xElement = XElement.Parse(xmlDoc.GetXml());
			foreach (var element in xElement.Elements("Template"))
			{
				this.Add(LbTemplate.FromXml(element));
			}
		}
	}
}
