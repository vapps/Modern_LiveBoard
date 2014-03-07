using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml.Linq;
using Windows.UI;
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
		/// 제공된 페이지 데이터에 데이터를 입력한다.
		/// </summary>
		/// <param name="pageData"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public static LbPageData Parse(LbPageData pageData, string data)
		{
			switch (pageData.ValueType.Name.ToLower())
			{
				case "string":
					pageData.Data = data;
					break;
				case "int":
				case "integer":
					pageData.Data = int.Parse(data);
					break;
				case "double":
					pageData.Data = double.Parse(data);
					break;
				case "color":
					string colorcode = data.Replace("#", "");
					int argb = Int32.Parse(colorcode, NumberStyles.HexNumber);
					if (colorcode.Length > 6)
					{
						pageData.Data = Color.FromArgb((byte)((argb & -16777216) >> 0x18), // 0x18=24
							(byte)((argb & 0xff0000) >> 0x10), // 0x10=16
							(byte)((argb & 0xff00) >> 8),
							(byte)(argb & 0xff));
					}
					else
					{
						pageData.Data = Color.FromArgb(0xff,
							(byte)((argb & 0xff0000) >> 0x10), // 0x10=16
							(byte)((argb & 0xff00) >> 8),
							(byte)(argb & 0xff));
					}
					break;
				default:
					// typeof(IEnumerable<string>) 이것이 변환. System.Collections.Generic.IEnumerable`1[System.String]
					pageData.Data = new ObservableCollection<string>();
					break;
			}
			return pageData;
		}

		/// <summary>
		/// XML에서 인스턴스 생성.
		/// </summary>
		/// <param name="xElement"></param>
		/// <returns></returns>
		public static LbPageData FromXml(XElement xElement)
		{
			// <Data Key="Url" Name="Header" ValueType="String" DefaultData="" />

			var tData = new LbPageData
			{
				Key = xElement.Attribute("Key").Value,
				Name = xElement.Attribute("Name").Value,
				IsHidden = bool.Parse(xElement.Attribute("IsHidden") != null
					? xElement.Attribute("IsHidden").Value
					: bool.FalseString)
			};
			switch (xElement.Attribute("ValueType").Value.ToLower())
			{
				case "string":
					tData.ValueType = typeof(string);
					tData.DefaultData = xElement.Attribute("DefaultValue").Value;
					break;
				case "int":
				case "integer":
					tData.ValueType = typeof(int);
					tData.DefaultData = int.Parse(!String.IsNullOrEmpty(xElement.Attribute("DefaultValue").Value) ? xElement.Attribute("DefaultValue").Value : "0");
					break;
				case "double":
					tData.ValueType = typeof(double);
					tData.DefaultData = double.Parse(!String.IsNullOrEmpty(xElement.Attribute("DefaultValue").Value) ? xElement.Attribute("DefaultValue").Value : "0");
					break;
				case "color":
					tData.ValueType = typeof(Color);
					string colorcode = xElement.Attribute("DefaultValue").Value.Replace("#", "");
					int argb = Int32.Parse(colorcode, NumberStyles.HexNumber);
					if (colorcode.Length > 6)
					{
						tData.DefaultData = Color.FromArgb((byte)((argb & -16777216) >> 0x18), // 0x18=24
							(byte)((argb & 0xff0000) >> 0x10), // 0x10=16
							(byte)((argb & 0xff00) >> 8),
							(byte)(argb & 0xff));
					}
					else
					{
						tData.DefaultData = Color.FromArgb(0xff,
							(byte)((argb & 0xff0000) >> 0x10), // 0x10=16
							(byte)((argb & 0xff00) >> 8),
							(byte)(argb & 0xff));
					}
					break;
				default:
					// typeof(IEnumerable<string>) 이것이 변환. System.Collections.Generic.IEnumerable`1[System.String]
					tData.ValueType = Type.GetType(xElement.Attribute("ValueType").Value);
					tData.DefaultData = new ObservableCollection<string>();
					break;
			}
			return tData;
		}

		/// <summary>
		/// XML로 정보 출력.
		/// </summary>
		/// <param name="isVerbose"><c>true</c>, 모든 정보 출력. <c>false</c>, Key와 Data만 출력.</param>
		/// <returns></returns>
		public XElement ToXml(bool isVerbose = false)
		{
			XElement xElement;
			if (!isVerbose)
			{
				xElement = new XElement("Data",
					new XAttribute("Key", Key ?? ""),
					new XAttribute("Data", Data.ToString() ?? "")
					);
			}
			else
			{
				xElement = new XElement("Data",
					new XAttribute("Key", Key ?? ""),
					new XAttribute("Data", Data.ToString() ?? ""),
					new XAttribute("DefaultValue", DefaultData.ToString() ?? ""),
					new XAttribute("ValueType", ValueType.Name ?? "String"),
					new XAttribute("IsHidden", IsHidden.ToString()),
					new XAttribute("Name", Name ?? "")
					);
			}
			return xElement;
		}
	}
}