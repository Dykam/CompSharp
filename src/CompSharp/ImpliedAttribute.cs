using System;
using JetBrains.Annotations;

namespace CompSharp
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    [MeansImplicitUse]
    public class ImpliedAttribute : Attribute
    {
    }
}
