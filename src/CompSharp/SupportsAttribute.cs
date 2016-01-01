using System;

namespace CompSharp
{
    public class SupportsAttribute : Attribute
    {
        public Type[] Types { get; set; }
        public SupportsAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}
