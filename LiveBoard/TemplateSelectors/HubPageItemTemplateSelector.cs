using System;
using System.Diagnostics;
using LiveBoard.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LiveBoard.TemplateSelectors
{
    public class HubPageItemTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            try
            {
                var DataTemplateForItem = XamlResourceHelper.GetItemTemplateFromPage(container, item.GetTemplateName());

                if (DataTemplateForItem != null)
                {
                    return DataTemplateForItem;
                }
                else
                {
                    Debug.WriteLine(String.Format("HubPageItemTemplateSelector failed to find the right template for object'{0}'", item.ToString()));

                    return base.SelectTemplateCore(item, container);
                }
            }
            catch (System.Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("Error in HubPageItemTemplateSelector.SelectTemplateCore, details: " + ex);
                    Debugger.Break();
                }

                return base.SelectTemplateCore(item, container);
            }
        }
    }
}
