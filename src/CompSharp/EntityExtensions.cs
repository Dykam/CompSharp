using System.Collections.Generic;

namespace CompSharp
{
    public static class EntityExtensions
    {
        public static IEntity Augment<T>(this IEntity entity, object parameters) =>
            entity.Augment().Complete<T>(parameters);
        public static IEntity Augment<T1, T2>(this IEntity entity, object parameters1 = null, object parameters2 = null) =>
            entity.Augment().Complete<T1, T2>(parameters1, parameters2);

        public static IEntity Complete<T1>(this IEntity entity, object parameters = null) =>
            entity.Complete(typeof (T1), parameters);

        public static IEntity Complete<T1, T2>(this IEntity entity, object parameters1 = null, object parameters2 = null)
            =>
                entity.Complete<T1>(parameters1).Complete<T2>(parameters2);

        public static IEntity Complete<T1, T2, T3>(this IEntity entity, object parameters1 = null,
            object parameters2 = null, object parameters3 = null) =>
                entity.Complete<T1>(parameters1).Complete<T2>(parameters2).Complete<T3>(parameters3);

        public static IEntity Complete<T1, T2, T3, T4>(this IEntity entity, object parameters1 = null,
            object parameters2 = null, object parameters3 = null, object parameters4 = null) =>
                entity.Complete<T1>(parameters1)
                    .Complete<T2>(parameters2)
                    .Complete<T3>(parameters3)
                    .Complete<T4>(parameters4);

        public static IEntity Complete<T1, T2, T3, T4, T5>(this IEntity entity, object parameters1 = null,
            object parameters2 = null, object parameters3 = null, object parameters4 = null, object parameters5 = null)
            =>
                entity.Complete<T1>(parameters1)
                    .Complete<T2>(parameters2)
                    .Complete<T3>(parameters3)
                    .Complete<T4>(parameters4)
                    .Complete<T5>(parameters5);

        public static T Get<T>(this IEntity entity) where T : class => entity.Get(typeof(T)) as T;
        public static IEnumerable<T> GetAll<T>(this IEntity entity) where T : class => entity.GetAll(typeof(T)) as IEnumerable<T>;
    }
}
