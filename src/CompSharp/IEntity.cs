using System;

namespace CompSharp
{
    public interface IEntity
    {
        Guid ID { get; }
        Component Get(Type componentType);
        IEntity Complete(Type componentType, object parameters = null);
        IEntity Augment();
    }
}
