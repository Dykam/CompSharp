namespace CompSharp
{
    public static class EntityHostExtensions
    {
        public static IEntity Create<T1>(this IEntityHost host, object parameters = null)
            => host.Create().Complete<T1>(parameters);

        public static IEntity Create<T1, T2>(this IEntityHost host, object parameters1 = null, object parameters2 = null)
            => host.Create().Complete<T1>(parameters1).Complete<T2>(parameters2);

        public static IEntity Create<T1, T2, T3>(this IEntityHost host, object parameters1 = null,
            object parameters2 = null, object parameters3 = null)
            => host.Create().Complete<T1, T2, T3>(parameters1, parameters2, parameters3);

        public static IEntity Create<T1, T2, T3, T4>(this IEntityHost host, object parameters1 = null,
            object parameters2 = null, object parameters3 = null, object parameters4 = null)
            => host.Create().Complete<T1, T2, T3, T4>(parameters1, parameters2, parameters3, parameters4);

        public static IEntity Create<T1, T2, T3, T4, T5>(this IEntityHost host, object parameters1 = null,
            object parameters2 = null, object parameters3 = null, object parameters4 = null, object parameters5 = null)
            =>
                host.Create()
                    .Complete<T1, T2, T3, T4, T5>(parameters1, parameters2, parameters3, parameters4, parameters5);
    }
}