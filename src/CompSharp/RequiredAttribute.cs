using System;

namespace CompSharp
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredAttribute : Attribute
    {
    }
}
