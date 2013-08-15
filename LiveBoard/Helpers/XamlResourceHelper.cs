using System.Collections.ObjectModel;
using System.Diagnostics;
using LiveBoard.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace LiveBoard.Helpers
{
    public static class XamlResourceHelper
    {
        public static DataTemplate GetItemTemplateFromPage(DependencyObject container, string ItemKey)
        {
            return GetResourceFromPage(container, ItemKey);
        }

        public static DataTemplate GetGroupDateTemplateFromPage(object item, DependencyObject container)
        {
            DataTemplate output = null;

            // Figure out the group key name
            var GroupKey = item as GroupInfoList<object>;
            if (GroupKey == null)
            {
                Debug.WriteLine("MainPageGroupTemplateSelector cannot determine template since IGroupKey interface is missing");
                return null;
            }

            string ItemKey = GroupKey.Key;

            // Get the datatemplate from resource dictionary within the host page
            output = XamlResourceHelper.GetResourceFromPage(container, ItemKey + "Group");

            return output;
        }

        public static string GetGroupKeyNameFromObject(DependencyObject item)
        {
            string output = string.Empty;

            if (item != null && item is ICollectionViewGroup)
            {

                var groupView = item as ICollectionViewGroup;
                var groupCollection = groupView.Group as GroupInfoList<object>;

                output = groupCollection.Key;
            }

            return output;
        }

        public static Style GetGroupContainerStyleFromPage(object item, DependencyObject container)
        {
            Style output = null;

            string GroupKey = string.Empty;

            if (item != null && item is GroupInfoList<object>)
            {
                GroupKey = ((GroupInfoList<object>)item).Key;
            }
            else if (item != null && item is ICollectionViewGroup)
            {
                var groupView = item as ICollectionViewGroup;
                var groupCollection = groupView.Group as GroupInfoList<object>;

                GroupKey = groupCollection.Key;
            }
            else
            {
                Debug.WriteLine("MainPageGroupTemplateSelector cannot determine the item type to retrieve the key");
                return null;
            }

            // Get the datatemplate from resource dictionary within the host page
            output = XamlResourceHelper.GetStyleFromPage(container, GroupKey + "Container");

            return output;
        }

        private static DataTemplate GetResourceFromPage(DependencyObject container, string ResourceName)
        {
            return GetDataTemplateFromPage(container, ResourceName, "DataTemplate") as DataTemplate;
        }

        private static Style GetStyleFromPage(DependencyObject container, string ResourceName)
        {
            return GetDataTemplateFromPage(container, ResourceName, "Style") as Style;
        }

        private static GroupStyle GetGroupStyleFromPage(DependencyObject container, string ResourceName)
        {
            return GetDataTemplateFromPage(container, ResourceName, "GroupStyle") as GroupStyle;
        }

        private static object GetDataTemplateFromPage(DependencyObject container, string TemplateName, string TemplateSuffix)
        {
            object output = null;

			Windows.UI.Xaml.Controls.Page ParentPage = container.FindParentPage() as Page; // Note: Have to use the FindParentPage since App.Current.Resources does not contain items from page

            // Create proper template name
            string NameOfResource = TemplateName + TemplateSuffix;
            Debug.WriteLine("GetDataTemplateFromPage is looking for a template with name " + NameOfResource);

            if (ParentPage != null && ParentPage.Resources.Count > 0)
            {
                // See if it exists based on key
                if (ParentPage.Resources.ContainsKey(NameOfResource))
                {
                    // If it does, get it and return it
                    var ResourceObject = ParentPage.Resources[NameOfResource] as object;

                    output = ResourceObject;
                }
            }

            if (output == null)
            {
                // Finaly attempt, try looking in global resource dictionary
                try
                {
                    Debug.WriteLine("Looking for style in global resource dictionary, key: " + NameOfResource);

                    output = Application.Current.Resources[NameOfResource] as object;
                }
                catch (System.Exception)
                {
                    // Empty by design
                }
            }

            return output;
        }
    }
}
