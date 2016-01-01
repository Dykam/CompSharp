using System;
using JetBrains.Annotations;

namespace CompSharp
{

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true)]
    [MeansImplicitUse]
    public class SupportAttribute : Attribute
    {
    }
}