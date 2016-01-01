using System;
using System.Collections.Generic;
using System.Linq;
using CompSharp.BuiltIn;
using CompSharp.Meta;

namespace CompSharp.InMemory
{
    internal class InMemoryEntity : IEntity
    {
        private readonly LinkedList<Component> _components;
        private readonly InMemoryEntity _original;

        internal InMemoryEntity(Guid id, InMemoryEntity original = null)
        {
            ID = id;
            _original = original;
            _components = new LinkedList<Component>();
        }

        public Guid ID { get; }

        public Component Get(Type componentType) =>
            _components.FirstOrDefault(componentType.IsInstanceOfType) ?? _original?.Get(componentType);

        public IEntity Augment() =>
            new InMemoryEntity(Guid.NewGuid(), this);

        public IEntity Complete(Type componentType, object parameters = null)
        {
            var component = ComponentInfo.For(componentType).Create(this, parameters);
            foreach (var other in _components)
                ComponentInfo.For(other.GetType()).GetSupport(componentType)(other, component);
            _components.AddFirst(component);
            return this;
        }

        public override string ToString()
        {
            return this.Get<IToString>()?.ToString() ?? ID.ToString();
        }

        protected bool Equals(InMemoryEntity other)
        {
            return Equals(_components, other._components) && Equals(_original, other._original) && ID.Equals(other.ID);
        }

        public override bool Equals(object obj)
        {
            var equalsComponent = this.Get<IEquals>();
            if (equalsComponent != null)
                return equalsComponent.Equals(obj);

            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((InMemoryEntity) obj);
        }

        public override int GetHashCode()
        {
            var getHashCodeComponent = this.Get<IGetHashCode>();
            if (getHashCodeComponent != null)
                return getHashCodeComponent.GetHashCode();
            unchecked
            {
                var hashCode = _components?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ (_original?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ ID.GetHashCode();
                return hashCode;
            }
        }
    }
}
