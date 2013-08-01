using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace LiveBoard
{
    public static class DependencyObjectExtensions
    {
        public static DependencyObject FindParentPage(this DependencyObject child)
        {
            var parent = VisualTreeHelper.GetParent(child);

            if (parent == null)
            {
                return null;
            }

            if (parent is Page)
            {
                return parent;
            }
            else
            {
                return parent.FindParentPage();
            }
        }
    }
}
