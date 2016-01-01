using System;

namespace CompSharp
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    public class ImpliesAttribute : Attribute
    {
        public Type[] Types { get; set; }
        public ImpliesAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}
