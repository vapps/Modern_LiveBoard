using System;

namespace LiveBoard.Attributes
{
    [AttributeUsage(System.AttributeTargets.Class | AttributeTargets.Struct)]
    public class TemplateItemName : Attribute
    {
        public string Name { get; private set; }

        public TemplateItemName(string Name)
        {
            this.Name = Name;
        }
    }
}
