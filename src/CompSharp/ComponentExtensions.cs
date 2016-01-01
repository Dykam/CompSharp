using CompSharp.Meta;

namespace CompSharp
{
    public static class ComponentExtensions
    {
        public static ComponentInfo GetInfo<T>(this T component) where T : Component =>
            ComponentInfo.For<T>();
    }
}
