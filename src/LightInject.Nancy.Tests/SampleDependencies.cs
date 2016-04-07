namespace LightInject.Nancy.Tests
{
    using System;
    using global::Nancy;
    using Xunit;

    public interface ITransient
    {
        
    }
    
    public class Transient : ITransient
    {
    }

    public class DisposableTransient :IDisposable
    {
        public void Dispose()
        {            
        }
    }

    public interface ISingleton
    {
        
    }

    public class Singleton : ISingleton
    {

    }

    public interface IPerRequest
    {
        
    }

    public class PerRequest : IPerRequest
    {
    }

    public interface ICollectionTypeSingleton
    {

    }

    public class CollectionTypeSingleton1 : ICollectionTypeSingleton
    {

    }

    public class CollectionTypeSingleton2 : ICollectionTypeSingleton
    {

    }

    public interface ICollectionTypePerRequest
    {

    }

    public class CollectionTypePerRequest1 : ICollectionTypePerRequest
    {

    }

    public class CollectionTypePerRequest2 : ICollectionTypePerRequest
    {

    }


    public interface ICollectionTypeTransient
    {

    }

    public class CollectionTypeTransient1 : ICollectionTypeTransient
    {

    }

    public class CollectionTypeTransient2 : ICollectionTypeTransient
    {

    }
}