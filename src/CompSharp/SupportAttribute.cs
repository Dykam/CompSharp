using System;

namespace CompSharp
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    public class SupportAttribute : Attribute
    {
    }
}