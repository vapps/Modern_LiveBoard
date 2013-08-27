using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiveBoard.PageTemplate.Model;

namespace LiveBoard.Model
{
	public class LbPage
	{
		public IPage Page { get; set; }
		public LbTemplate Template { get; set; }
		public ObservableCollection<LbTemplateData> TemplateData { get; set; } 
	}
}
