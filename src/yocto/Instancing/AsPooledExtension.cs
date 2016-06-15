//using System;

//namespace yocto
//{
//    public static class AsPooledExtension
//    {
//        public static IRegistration AsPooled(this IRegistration registration)
//        {
//            return registration.Register(Instancing.InstancePerThread);
//        }

//        public static IRegistration RegisterPooled<T, V>(this IContainer container) where V : T
//        {
//            return container.Register<T, V>().AsPooled();
//        }
//    }
//}
