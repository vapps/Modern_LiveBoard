using System.Diagnostics;
using LiveBoard.Helpers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LiveBoard.TemplateSelectors
{
    public class HubPageContainerStyleSelector : StyleSelector
    {
        public Style HubGroupContainerStyleDefault { get; set; }

        protected override Style SelectStyleCore(object item, DependencyObject container)
        {
            try
            {
                var GroupContainerDataTemplate = XamlResourceHelper.GetGroupContainerStyleFromPage(item, container);

                if (GroupContainerDataTemplate != null)
                {
                    return GroupContainerDataTemplate;
                }
                else
                {
                    return HubGroupContainerStyleDefault;
                }
            }
            catch (System.Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    Debug.WriteLine("Error in HubPageContainerStyleSelector.SelectStyleCore, details: " + ex);
                    Debugger.Break();
                }

                return base.SelectStyleCore(item, container);
            }
        }
    }
}
