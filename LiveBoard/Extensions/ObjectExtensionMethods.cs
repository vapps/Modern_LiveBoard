using System;
using System.Reflection;
using LiveBoard.Attributes;
using LiveBoard.Controls;

namespace LiveBoard
{
    public static class ObjectExtensionMethods
    {
        public static string GetTemplateName(this object item)
        {
            string output = string.Empty;

            // Try to get name from attribute
            System.Reflection.MemberInfo info = item.GetType().GetTypeInfo();
            foreach (object attrib in info.GetCustomAttributes(true))
            {
                if (attrib is TemplateItemName)
                {
                    var TemplateNameAttribute = attrib as TemplateItemName;

                    output = TemplateNameAttribute.Name;
                }
            }

            // If not found in attribute, try to get name from Type (class name)
            if (output == string.Empty)
            {
                output = item.GetType().GetTypeInfo().Name;
            }

            if (item is IVariableGridSize)
            {
                var itemWithSize = item as IVariableGridSize;

                if (itemWithSize.RowSpan > 1 || itemWithSize.ColumnSpan > 1)
                {
                    output = output + String.Format("{0}by{1}", itemWithSize.RowSpan, itemWithSize.ColumnSpan);
                }
            }

            return output;
        }
    }
}
