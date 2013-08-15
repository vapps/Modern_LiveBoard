using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LiveBoard.TemplateSelectors
{
    public class HubPageGroupStyleSelector : GroupStyleSelector
    {
        public GroupStyle DefaultGroupStyle { get; set; }

        protected override GroupStyle SelectGroupStyleCore(object group, uint level)
        {
            return DefaultGroupStyle;
        }
    }
}
