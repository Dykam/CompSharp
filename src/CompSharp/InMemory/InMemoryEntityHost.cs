using System;

namespace CompSharp.InMemory
{
    public class InMemoryEntityHost : IEntityHost
    {
        public IEntity Create()
        {
            return new InMemoryEntity(Guid.NewGuid());
        }
    }
}
