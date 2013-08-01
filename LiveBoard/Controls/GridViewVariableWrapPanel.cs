using System.Diagnostics;
using Windows.UI.Xaml.Controls;

namespace LiveBoard.Controls
{
    /// <summary>
    /// Should be implemented in any class that wants to control its visual size during template selection
    /// </summary>
    public interface IVariableGridSize
    {
        int RowSpan { get; set; }
        int ColumnSpan { get; set; }
    }

    /// <summary>
    /// GridViewVariableWrapPanel is courtesy of Jerry Nixon's (Microsoft DPE) Sample, for more info see:
    /// http://blog.jerrynixon.com/2012/08/windows-8-beauty-tip-using.html
    /// </summary>
    public class GridViewVariableWrapPanel : GridView
    {
        [DebuggerNonUserCode] // to avoid showing first chance exceptions in Output window - Exceptions are expected below & its normal
        protected override void PrepareContainerForItemOverride(Windows.UI.Xaml.DependencyObject element, object item)
        {
            try
            {
                if (item is IVariableGridSize)
                {
                    dynamic _Item = item;
                    element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, _Item.ColumnSpan);
                    element.SetValue(VariableSizedWrapGrid.RowSpanProperty, _Item.RowSpan);
                }
            }
            catch // Ignoring Exceptions here is by design
            {
                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 1);
                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 1);
            }
            finally
            {
                base.PrepareContainerForItemOverride(element, item);
            }
        }
    }
}
