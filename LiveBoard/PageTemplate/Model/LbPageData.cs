using System;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using GalaSoft.MvvmLight;

namespace LiveBoard.PageTemplate.Model
{
	/// <summary>
	/// 템플릿 데이터
	/// </summary>
	public class LbPageData : ObservableObject
	{
		private object _data;
		private string _name;
		private string _key;
		private object _defaultData;
		private bool _isHidden;
		private Type _valueType;

		public string Key
		{
			get { return _key; }
			set
			{
				_key = value;
				RaisePropertyChanged("Key");
			}
		}

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				RaisePropertyChanged("Name");
			}
		}

		public Type ValueType
		{
			get { return _valueType; }
			set
			{
				_valueType = value;
				RaisePropertyChanged("ValueType");
			}
		}

		public object Data
		{
			get
			{
				if (_data == null)
					return DefaultData;
				return _data;
			}
			set
			{
				_data = value;
				RaisePropertyChanged("Data");
			}
		}

		public object DefaultData
		{
			get { return _defaultData; }
			set
			{
				_defaultData = value;
				RaisePropertyChanged("DefaultData");
			}
		}

		public bool IsHidden
		{
			get { return _isHidden; }
			set
			{
				_isHidden = value;
				RaisePropertyChanged("IsHidden");
			}
		}

		/// <summary>
		/// XML에서 인스턴스 생성.
		/// </summary>
		/// <param name="xElement"></param>
		/// <returns></returns>
		public static LbPageData FromXml(XElement xElement)
		{
			// <Data Key="Url" Name="Header" ValueType="String" DefaultData="" />

			var tData = new LbPageData { Key = xElement.Attribute("Key").Value, Name = xElement.Attribute("Name").Value };

			switch (xElement.Attribute("ValueType").Value.ToLower())
			{
				case "string":
					tData.ValueType = typeof(string);
					tData.DefaultData = xElement.Attribute("DefaultValue").Value;
					break;
				case "int":
				case "integer":
					tData.ValueType = typeof(int);
					tData.DefaultData = int.Parse(xElement.Attribute("DefaultValue").Value);
					break;
				case "double":
					tData.ValueType = typeof(double);
					tData.DefaultData = double.Parse(xElement.Attribute("DefaultValue").Value);
					break;
				default:
					// typeof(IEnumerable<string>) 이것이 변환. System.Collections.Generic.IEnumerable`1[System.String]
					tData.ValueType = Type.GetType(xElement.Attribute("ValueType").Value);
					tData.DefaultData = new ObservableCollection<string>();
					break;
			}
			return tData;
		}
	}
}