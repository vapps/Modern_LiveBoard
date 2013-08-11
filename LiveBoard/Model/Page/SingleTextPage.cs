using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using LiveBoard.Common;
using Newtonsoft.Json;

namespace LiveBoard.Model.Page
{
    /// <summary>
    /// 페이지 인터페이스
    /// </summary>
    public class SingleTextPage: ObservableObject, IPage
    {
        public SingleTextPage ()
        {
        }

        public string Guid { get; set; }
        public string Title { get; set; }
        public string TemplateCode { get; set; }
        public string TemplateOption { get; set; }
        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
        public bool IsVisible { get; set; }

        /// <summary>
        /// Specific data.
        /// </summary>
        public string Data
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
            return String.Format("{0} ({1})", Title, TemplateCode);
        }

        private string _data;
    }
}
