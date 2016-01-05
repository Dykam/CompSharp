using System;
using System.Collections.Generic;

namespace CompSharp
{
    public interface IEntity
    {
        Guid ID { get; }
        Component Get(Type componentType);
        IEnumerable<Component> GetAll(Type componentType);
        IEntity Complete(Type componentType, object parameters = null);
        IEntity Augment();
    }
}
