namespace LightInject.Nancy.Tests
{
    using System;
    using System.Collections.Generic;

    using global::Nancy;
    using Xunit;

    public class SampleModule : NancyModule
    {
        public SampleModule()
        {
            
        }
    }

    public class SampleModuleWithNancyContextDependency : NancyModule
    {
        private readonly Func<NancyContext> getNancyContext;


        public SampleModuleWithNancyContextDependency(Func<NancyContext> getNancyContext)
        {
            Get("/", _ =>
            {
                var context = getNancyContext();
                Assert.NotNull(context);
                return string.Empty;
            });

        }
    }


    public class SampleModuleWithTransientDependency : NancyModule
    {
        public ITransient Transient { get; private set; }

        public SampleModuleWithTransientDependency(ITransient transient)
        {
            Transient = transient;
        }
    }

    public class SampleModuleWithDisposableTransientDependency : NancyModule
    {
        public IDisposable Transient { get; private set; }

        public SampleModuleWithDisposableTransientDependency(IDisposable transient)
        {
            Transient = transient;
        }
    }

    public class SampleModuleWithPerRequestDependency : NancyModule
    {
        public IPerRequest PerRequest { get; private set; }

        public SampleModuleWithPerRequestDependency(IPerRequest perRequest)
        {
            PerRequest = perRequest;
        }
    }

    public class SampleModuleWithSingletonDependency : NancyModule
    {
        public ISingleton Singleton { get; private set; }

        public SampleModuleWithSingletonDependency(ISingleton singleton)
        {
            Singleton = singleton;
        }
    }

    public class SampleModuleWithPerRequestCollectionDependency : NancyModule
    {
        public IEnumerable<ICollectionTypePerRequest> Instances { get; private set; }

        public SampleModuleWithPerRequestCollectionDependency(IEnumerable<ICollectionTypePerRequest> instances)
        {
            Instances = instances;           
        }
    }


    public class SampleModuleWithTransientCollectionDependency : NancyModule
    {
        public IEnumerable<ICollectionTypeTransient> Instances { get; private set; }

        public SampleModuleWithTransientCollectionDependency(IEnumerable<ICollectionTypeTransient> instances)
        {
            this.Instances = instances;           
        }
    }


    public class SampleModuleWithSingletonCollectionDependency : NancyModule
    {
        public IEnumerable<ICollectionTypeSingleton> Instances { get; private set; }

        public SampleModuleWithSingletonCollectionDependency(IEnumerable<ICollectionTypeSingleton> instances)
        {
            this.Instances = instances;
        }
    }


}