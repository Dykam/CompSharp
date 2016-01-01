//using System;
//
//namespace CompSharp.Meta
//{
//    internal static class Utils
//    {
//        private static Type GetActionTypeFor(Type[] paramTypes)
//        {
//            switch (paramTypes.Length)
//            {
//                case 0:
//                    return typeof (Action);
//                case 1:
//                    return typeof (Action<>).MakeGenericType(paramTypes);
//                case 2:
//                    return typeof (Action<,>).MakeGenericType(paramTypes);
//                case 3:
//                    return typeof (Action<,,>).MakeGenericType(paramTypes);
//                case 4:
//                    return typeof (Action<,,,>).MakeGenericType(paramTypes);
//                case 5:
//                    return typeof (Action<,,,,>).MakeGenericType(paramTypes);
//                case 6:
//                    return typeof (Action<,,,,,>).MakeGenericType(paramTypes);
//                case 7:
//                    return typeof (Action<,,,,,,>).MakeGenericType(paramTypes);
//                case 8:
//                    return typeof (Action<,,,,,,,>).MakeGenericType(paramTypes);
//                case 9:
//                    return typeof (Action<,,,,,,,,>).MakeGenericType(paramTypes);
//                case 10:
//                    return typeof (Action<,,,,,,,,,>).MakeGenericType(paramTypes);
//                case 11:
//                    return typeof (Action<,,,,,,,,,,>).MakeGenericType(paramTypes);
//                case 12:
//                    return typeof (Action<,,,,,,,,,,,>).MakeGenericType(paramTypes);
//                case 13:
//                    return typeof (Action<,,,,,,,,,,,,>).MakeGenericType(paramTypes);
//                case 14:
//                    return typeof (Action<,,,,,,,,,,,,,>).MakeGenericType(paramTypes);
//                case 15:
//                    return typeof (Action<,,,,,,,,,,,,,,>).MakeGenericType(paramTypes);
//                case 16:
//                    return typeof (Action<,,,,,,,,,,,,,,,>).MakeGenericType(paramTypes);
//                default:
//                    throw new NotSupportedException($"No Action exists for {paramTypes.Length} parameters");
//            }
//        }
//    }
//}
