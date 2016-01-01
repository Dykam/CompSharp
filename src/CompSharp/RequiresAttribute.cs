using System;

namespace CompSharp
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    public class RequiresAttribute : Attribute
    {
        public Type[] Types { get; set; }
        public RequiresAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}
