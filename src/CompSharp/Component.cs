namespace CompSharp
{
    public abstract class Component
    {
        protected internal IEntity Entity { get; internal set; }

        /// <summary>
        /// Called after all components and initialization values are injected, but before Support
        /// methods are invoked for any existing supported Components
        /// </summary>
        protected internal virtual void Initialize() { }
        /// <summary>
        /// Called after all components are injected, but before initialization values are injected
        /// and Support methods are invoked for any existing supported Components
        /// </summary>
        protected internal virtual void InitializeEarly() { }
    }
}
