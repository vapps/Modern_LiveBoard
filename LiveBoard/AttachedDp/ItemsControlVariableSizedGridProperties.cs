using Windows.UI.Xaml;

namespace LiveBoard.AttachedDp
{
    public class ItemsControlVariableSizedGridProperties
    {
        #region ItemWidth Attached Dependency Property
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.RegisterAttached(
                "ItemWidth", typeof(int), typeof(ItemsControlVariableSizedGridProperties),
                new PropertyMetadata(0));

        public static void SetItemWidth(DependencyObject attached, int value)
        {
            attached.SetValue(ItemWidthProperty, value);
        }

        public static int GetItemWidth(DependencyObject attached)
        {
            return (int)attached.GetValue(ItemWidthProperty);
        }
        #endregion

        #region ItemHeight Attached Dependency Property
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.RegisterAttached(
                "ItemHeight", typeof(int), typeof(ItemsControlVariableSizedGridProperties),
                new PropertyMetadata(0));

        public static void SetItemHeight(DependencyObject attached, int value)
        {
            attached.SetValue(ItemHeightProperty, value);
        }

        public static int GetItemHeight(DependencyObject attached)
        {
            return (int)attached.GetValue(ItemHeightProperty);
        }
        #endregion

        #region MaximumRowsOrColumns Attached Dependency Property
        public static readonly DependencyProperty MaximumRowsOrColumnsProperty =
            DependencyProperty.RegisterAttached(
                "MaximumRowsOrColumns", typeof(int), typeof(ItemsControlVariableSizedGridProperties),
                new PropertyMetadata(0));

        public static void SetMaximumRowsOrColumns(DependencyObject attached, int value)
        {
            attached.SetValue(MaximumRowsOrColumnsProperty, value);
        }

        public static int GetMaximumRowsOrColumns(DependencyObject attached)
        {
            return (int)attached.GetValue(MaximumRowsOrColumnsProperty);
        }
        #endregion
    }
}
