using System;

namespace iLevel.CodeAnalysis.BestPractices.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Delegate | AttributeTargets.Enum | AttributeTargets.Event | AttributeTargets.Field | AttributeTargets.Interface
        | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Struct,
        AllowMultiple = true)
    ]
    public class BoundedContext : Attribute
    {
        public string namespaceString;

        public BoundedContext(string namespaceString)
        {
            this.namespaceString = namespaceString;
        }
    }
}
