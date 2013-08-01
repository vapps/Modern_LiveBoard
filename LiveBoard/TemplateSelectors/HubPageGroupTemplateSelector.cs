using System.Diagnostics;
using LiveBoard.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LiveBoard.TemplateSelectors
{
    public class HubPageGroupTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            try
            {
                var GroupDataTemplate = XamlResourceHelper.GetGroupDateTemplateFromPage(item, container);

                if (GroupDataTemplate != null)
                {
                    return GroupDataTemplate;
                }
                else
                {
                    Debug.WriteLine("HubPageGroupTemplateSelector cannot determine template");

                    return base.SelectTemplateCore(item, container);
                }
            }
            catch (System.Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("Error in HubPageGroupTemplateSelector.SelectTemplateCore, details: " + ex);
                    Debugger.Break();
                }

                return base.SelectTemplateCore(item, container);
            }
        }
    }
}
