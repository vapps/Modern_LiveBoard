﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using GalaSoft.MvvmLight;
using LiveBoard.PageTemplate.Model;
using Newtonsoft.Json;

namespace LiveBoard.Model
{
	/// <summary>
	/// 보드 오브젝트.
	/// </summary>
	public class Board : ObservableObject
	{
		/// <summary>
		/// Title of board
		/// </summary>
		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				RaisePropertyChanged("Title");
			}
		}

		/// <summary>
		/// Author name
		/// </summary>
		public string Author
		{
			get { return _author; }
			set
			{
				_author = value;
				RaisePropertyChanged("Author");
			}
		}

		/// <summary>
		/// Author contact point
		/// </summary>
		public string AuthorEmail
		{
			get { return _authorEmail; }
			set
			{
				_authorEmail = value;
				RaisePropertyChanged("AuthorEmail");
			}
		}

		/// <summary>
		/// [ReadOnly] Loop or run once.
		/// <para>[읽기전용] 단발성 실행 또는 무한루프.</para>
		/// <value>false</value> if <see cref="LoopCount"/> is 0. Otherwise, <value>true</value>. 
		/// </summary>
		[JsonIgnore]
		public bool IsLoop
		{
			get { return LoopCount != 0; }
			set
			{
				if (value)
					LoopCount = -1;
				RaisePropertyChanged("IsLoop");
				RaisePropertyChanged("LoopCount");
			}
		}

		/// <summary>
		/// Loop count.
		/// <para>Default is -1 (infinite)</para>
		/// </summary>
		public int LoopCount
		{
			get { return _loopCount; }
			set
			{
				_loopCount = value;
				RaisePropertyChanged("IsLoop");
				RaisePropertyChanged("LoopCount");
			}
		}


		/// <summary>
		/// Limit time to run
		/// </summary>
		public DateTime RunUntil
		{
			get { return _runUntil; }
			set
			{
				_runUntil = value;
				RaisePropertyChanged("RunUntil");
			}
		}


		/// <summary>
		/// List of page
		/// </summary>
		[JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
		public ObservableCollection<IPage> Pages
		{
			get { return _pages; }
			set
			{
				_pages = value;
				RaisePropertyChanged("Pages");
			}
		}


		#region Private Variables
		private ObservableCollection<IPage> _pages = new ObservableCollection<IPage>();
		private string _title;
		private string _author;
		private string _authorEmail;
		private bool _isLoop = true;
		private int _loopCount = -1; // default -1 (infinite)
		private DateTime _runUntil;

		#endregion Private Variables

		/// <summary>
		/// XML로 출력.
		/// </summary>
		/// <returns></returns>
		public XElement ToXml()
		{
			var xElement = new XElement("LiveBoard",
				new XAttribute("IsLoop", IsLoop),
				new XAttribute("Author", Author),
				new XAttribute("Title", Title),
				new XAttribute("AuthorEmail", AuthorEmail),
				new XElement("Pages", Pages.Select(x => x.ToXml())));
			return xElement;
		}

		
	}
}