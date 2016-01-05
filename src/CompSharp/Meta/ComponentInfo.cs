using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace CompSharp.Meta
{
    public class ComponentInfo
    {
        private readonly LinkedList<Type> _implies = new LinkedList<Type>();
        private readonly LinkedList<Type> _provides = new LinkedList<Type>();
        private readonly LinkedList<Type> _requires = new LinkedList<Type>();
        private readonly LinkedList<Type> _supports = new LinkedList<Type>();
        private readonly Dictionary<Type, List<Action<Component, Component>>> _injected = new Dictionary<Type, List<Action<Component, Component>>>();
        private readonly Dictionary<Type, List<Action<Component, Component>>> _supportInvokers = new Dictionary<Type, List<Action<Component, Component>>>();

        private readonly Type _componentType;
        private readonly Type _type;

        internal ComponentInfo(Type componentType)
        {
            if (!typeof (Component).IsAssignableFrom(componentType) || componentType.GetTypeInfo().IsAbstract)
                throw new ArgumentException("Type must be a component (inherit Component) and cannot be abstract",
                    nameof(componentType));

            _type = componentType;
            _componentType = componentType;

            ProcessClassAttributes();
            ProcessInjected();
            ProcessSupports();
        }

        private void ProcessInjected()
        {
            foreach (var prop in _type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var impliedAttribute = prop.GetCustomAttribute<ImpliedAttribute>();
                var requiredAttribute = prop.GetCustomAttribute<RequiredAttribute>();
                var supportAttribute = prop.GetCustomAttribute<SupportAttribute>();

                if (impliedAttribute == null && requiredAttribute == null && supportAttribute == null) continue;

                if (impliedAttribute != null && requiredAttribute != null && supportAttribute != null)
                    throw new InvalidOperationException(
                        "Property can't have both RequiredAttribute and ImpliedAttribute");

                (impliedAttribute != null ? _implies : requiredAttribute != null ? _requires : _supports).AddLast(prop.PropertyType);

                if (!_injected.ContainsKey(prop.PropertyType))
                    _injected.Add(prop.PropertyType, new List<Action<Component, Component>>());

                if (!prop.CanWrite)
                {
                    var attributeName = (impliedAttribute ?? requiredAttribute ?? (object)supportAttribute).GetType().Name;
                    throw new InvalidOperationException($"Property {prop.Name} has attribute {attributeName} but no setter was found");
                }

                _injected[prop.PropertyType].Add(prop.SetValue);

                if (supportAttribute == null) continue;
                if (!_supportInvokers.ContainsKey(prop.PropertyType))
                    _supportInvokers.Add(prop.PropertyType, new List<Action<Component, Component>>());
                _supportInvokers[prop.PropertyType].Add(prop.SetValue);
            }
        }

        private void ProcessSupports()
        {
            foreach (var method in _type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var supportsAttribute = method.GetCustomAttribute<SupportsAttribute>();

                if (supportsAttribute == null) continue;

                _supports.AddLast(method.ReturnType);

                if (!_supportInvokers.ContainsKey(method.ReturnType))
                    _supportInvokers.Add(method.ReturnType, new List<Action<Component, Component>>());
                _supportInvokers[method.ReturnType].Add((component, supported) => method.Invoke(component, new object[] { supported }));
            }
        }

        private void ProcessClassAttributes()
        {
            foreach (var impliedType in _type.GetTypeInfo().GetCustomAttributes<ImpliesAttribute>().SelectMany(attr => attr.Types))
                _implies.AddLast(impliedType);
            foreach (var requiredType in _type.GetTypeInfo().GetCustomAttributes<RequiresAttribute>().SelectMany(attr => attr.Types))
                _requires.AddLast(requiredType);
            foreach (var supportedType in _type.GetTypeInfo().GetCustomAttributes<SupportsAttribute>().SelectMany(attr => attr.Types))
                _supports.AddLast(supportedType);
            foreach (var providedType in _type.GetInterfaces())
                _provides.AddLast(providedType);
        }

        public IEnumerable<Type> Requires => _requires;

        public IEnumerable<Type> Supports => _supports;

        public IEnumerable<Type> Implies => _implies;

        public IEnumerable<Type> Provides => _provides;

        public Action<Component, T> GetInjector<T>() where T : Component => GetInjector(typeof (T));
        public Action<Component, Component> GetInjector(Type injectedComponentType)
            => (component, injectedComponent) =>
            {
                if (!_injected.ContainsKey(injectedComponentType)) return;
                foreach (var action in _injected[injectedComponentType])
                    action(component, injectedComponent);
            };

        public Action<Component, T> GetSupport<T>() where T : Component => GetSupport(typeof (T));
        public Action<Component, Component> GetSupport(Type supportedComponentType)
            => (component, supportedComponent) =>
            {
                foreach (var provided in For(supportedComponentType).Provides.Concat(new[] {supportedComponentType}))
                {
                    if (!_supportInvokers.ContainsKey(provided)) return;
                    foreach (var action in _supportInvokers[provided])
                        action(component, supportedComponent);
                }
            };

        public static ComponentInfo For<T>() => For(typeof (T));

        public static ComponentInfo For(Type componentType) => new ComponentInfo(componentType);


        public Component Create(IEntity entity, object parameters = null)
        {
            ValidateRequired(entity);
            EnsureImplied(entity);

            var component = (Component)Activator.CreateInstance(_componentType);
            component.Entity = entity;

            InjectComponents(entity, component);

            component.InitializeEarly();

            InjectParameters(parameters, component);

            component.Initialize();

            InvokeSupports(entity, component);

            return component;
        }

        private void InvokeSupports(IEntity entity, Component component)
        {
            foreach (var supportInvoker in _supportInvokers)
            {
                var obj = entity.Get(supportInvoker.Key);
                if (obj == null)
                    continue;
                foreach (var action in supportInvoker.Value)
                    action(component, obj);
            }
        }

        private void InjectParameters(object parameters, Component component)
        {
            if(parameters == null)
                return;

            foreach (var parameterProp in parameters.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!parameterProp.CanRead)
                    continue;

                var componentProp = _componentType.GetProperty(parameterProp.Name);
                if (!componentProp.CanWrite)
                    continue;

                componentProp.SetValue(component, parameterProp.GetValue(parameters));
            }
        }

        private void EnsureImplied(IEntity entity)
        {
            foreach (var implied in Implies)
                entity.Complete(implied);
        }

        private void ValidateRequired(IEntity entity)
        {
            foreach (var required in Requires.Where(required => entity.Get(required) == null))
                throw new InvalidOperationException(
                    $"{required.Name} is required by {_type.Name} but is missing on entity {entity.ID}");
        }

        private void InjectComponents(IEntity entity, Component component)
        {
            foreach (var injector in _injected)
            {
                var obj = entity.Get(injector.Key);
                if (obj == null)
                    continue;
                foreach (var action in injector.Value)
                    action(component, obj);
            }
        }
    }

    public class ComponentInfo<T> : ComponentInfo where T : Component
    {
        public static ComponentInfo Info { get; } = new ComponentInfo<T>();
        public new static T Create(IEntity entity, object parameters = null) => (T) Info.Create(entity, parameters);

        public ComponentInfo() : base(typeof(T))
        {
        }
    }

    public static class ComponentInfoExtensions
    {
        public static T Create<T>(this ComponentInfo<T> info, IEntity entity, object parameters = null)
            where T : Component => ComponentInfo<T>.Create(entity, parameters);
    }
}