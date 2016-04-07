namespace LightInject.Nancy.Tests
{
    using System;
    using System.Collections.Generic;

    using global::Nancy.Bootstrapper;

    public class SampleRegistrations : IRegistrations
    {
        public IEnumerable<TypeRegistration> TypeRegistrations
        {
            get
            {
                yield return new TypeRegistration(typeof(ITransient), typeof(Transient), Lifetime.Transient);
                yield return new TypeRegistration(typeof(IDisposable), typeof(DisposableTransient), Lifetime.Transient);
                yield return new TypeRegistration(typeof(IPerRequest), typeof(PerRequest), Lifetime.PerRequest);
                yield return new TypeRegistration(typeof(ISingleton), typeof(Singleton), Lifetime.Singleton);
            }
        }

        public IEnumerable<CollectionTypeRegistration> CollectionTypeRegistrations
        {
            get
            {
                yield return
                    CreateCollectionTypeRegistration
                        <ICollectionTypePerRequest, CollectionTypePerRequest1, CollectionTypePerRequest2>(
                            Lifetime.PerRequest);
                yield return
                    CreateCollectionTypeRegistration
                        <ICollectionTypeTransient, CollectionTypeTransient1, CollectionTypeTransient2>(
                            Lifetime.Transient);
                yield return
                    CreateCollectionTypeRegistration
                        <ICollectionTypeSingleton, CollectionTypeSingleton1, CollectionTypeSingleton2>(
                            Lifetime.Singleton);
            }
        }

        private static CollectionTypeRegistration CreateCollectionTypeRegistration<TServiceType, T1, T2>(
            Lifetime lifetime) where T1 : TServiceType where T2 : TServiceType
        {
            return new CollectionTypeRegistration(typeof(TServiceType), new []{typeof(T1), typeof(T2)}, lifetime);
        }


        public IEnumerable<InstanceRegistration> InstanceRegistrations { get; private set; }
    }

    
}