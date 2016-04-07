using Xunit;

namespace LightInject.Nancy.Tests
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using global::Nancy;
    using global::Nancy.Bootstrapper;
    using global::Nancy.Testing;

    
    public class LightInjectNancyBootstrapperTests
    {
        [Fact]
        public void GetEngine_ReturnsEngine()
        {
            var bootstrapper = new TestBootstrapper();
            bootstrapper.Initialise();
            var engine = bootstrapper.GetEngine();
            Assert.NotNull(engine);
        }

        [Fact]
        public void GetModule_ReturnsModule()
        {
            var bootstrapper = new TestBootstrapper();            
            bootstrapper.Initialise();
            bootstrapper.GetEngine();
            INancyModule module = bootstrapper.GetModule(typeof(SampleModule), new NancyContext());
            Assert.IsType(typeof(SampleModule), module);
        }

        [Fact]
        public void GetModule_TransientDependency_ReturnsModule()
        {
            var bootstrapper = new TestBootstrapper();
            bootstrapper.Initialise();
            bootstrapper.GetEngine();
            var module = (SampleModuleWithTransientDependency)bootstrapper.GetModule(
                typeof(SampleModuleWithTransientDependency),
                new NancyContext());
            Assert.IsType(typeof(Transient), module.Transient);
        }

        [Fact]
        public void GetModule_DisposableTransientDependency_ReturnsModule()
        {
            var bootstrapper = new TestBootstrapper();
            bootstrapper.Initialise();
            bootstrapper.GetEngine();
            var module = (SampleModuleWithDisposableTransientDependency)bootstrapper.GetModule(
                typeof(SampleModuleWithDisposableTransientDependency),
                new NancyContext());
            Assert.IsType(typeof(DisposableTransient), module.Transient);
        }


        [Fact]
        public void GetModule_PerRequestDependency_ReturnsModule()
        {
            var bootstrapper = new TestBootstrapper();
            bootstrapper.Initialise();
            bootstrapper.GetEngine();
            var module = (SampleModuleWithPerRequestDependency)bootstrapper.GetModule(
                typeof(SampleModuleWithPerRequestDependency),
                new NancyContext());
            Assert.IsType(typeof(PerRequest), module.PerRequest);
        }

        [Fact]
        public void GetAllModules_ReturnsModules()
        {
            var bootstrapper = new TestBootstrapper();
            bootstrapper.Initialise();
            bootstrapper.GetEngine();
            var modules = bootstrapper.GetAllModules(new NancyContext());
            Assert.True(modules.Any());
        }

        [Fact]
        public void GetEngine_RegistersRequestStartup()
        {
            var bootstrapper = new TestBootstrapper();
            bootstrapper.Initialise();
            var engine = bootstrapper.GetEngine();
            engine.HandleRequest(new Request("Post", "Sample", "http"));
        }

        [Fact]
        public void GetModule_WithPerRequestCollectionTypeDependency_ReturnsModuleWithDependencies()
        {
            var bootstrapper = CreateBootstrapper();                          
            var module = (SampleModuleWithPerRequestCollectionDependency)bootstrapper.GetModule(
                typeof(SampleModuleWithPerRequestCollectionDependency),
                new NancyContext());            

            Assert.Equal(2, module.Instances.Count());
        }

        [Fact]
        public void GetModule_WithTransientCollectionTypeDependency_ReturnsModuleWithDependencies()
        {
            var bootstrapper = CreateBootstrapper();
            var module = (SampleModuleWithTransientCollectionDependency)bootstrapper.GetModule(
                typeof(SampleModuleWithTransientCollectionDependency),
                new NancyContext());

            Assert.Equal(2, module.Instances.Count());
        }

        [Fact]
        public void GetModule_WithSingletonCollectionTypeDependency_ReturnsModuleWithDependencies()
        {
            var bootstrapper = CreateBootstrapper();
            var module = (SampleModuleWithSingletonCollectionDependency)bootstrapper.GetModule(
                typeof(SampleModuleWithSingletonCollectionDependency),
                new NancyContext());

            Assert.Equal(2, module.Instances.Count());
        }

        [Fact]
        public void GetModule_WithPerRequestCollectionTypeDependency_ReturnsDifferentDependencies()
        {           
            var bootstrapper = CreateBootstrapper();

            var firstModule = (SampleModuleWithPerRequestCollectionDependency)bootstrapper.GetModule(
                typeof(SampleModuleWithPerRequestCollectionDependency),
                new NancyContext());

            var secondModule = (SampleModuleWithPerRequestCollectionDependency)bootstrapper.GetModule(
                typeof(SampleModuleWithPerRequestCollectionDependency),
                new NancyContext());
            Assert.False(firstModule.Instances.SequenceEqual(secondModule.Instances));            
        }

        [Fact]
        public void Ping_ShouldInvokeRequestStartup()
        {
            var bootstrapper = new TestBootstrapper();            

            var browser = new Browser(bootstrapper);

            var response = browser.Get("/ping");
            
            Assert.True(response.StatusCode == HttpStatusCode.OK);
        }

        [Fact]
        public void GetModule_WithNancyContextDependency_ReturnsModuleWithDependency()
        {
            var bootstrapper = new TestBootstrapper();

            var browser = new Browser(bootstrapper);

            browser.Get("/");

        }



        private static TestBootstrapper CreateBootstrapper()
        {
            var bootstrapper = new TestBootstrapper();
            bootstrapper.Initialise();
            bootstrapper.GetEngine();
            return bootstrapper;
        }
    }

    internal class TestBootstrapper : LightInjectNancyBootstrapper
    {
        //public T GetInstance<T>()
        //{
        //    return InternalConfiguration.
        //}

        //protected override IServiceContainer GetServiceContainer()
        //{
        //    return container;
        //}

        //protected override void RegisterTypes(IServiceContainer container, IEnumerable<TypeRegistration> typeRegistrations)
        //{
        //    base.RegisterTypes(container, typeRegistrations);
        //}        
    }

    

    

    

    public class SampleRequestStartup : IRequestStartup
    {
        public void Initialize(IPipelines pipelines, NancyContext context)
        {
            //throw new NotImplementedException();
        }
    }

        

    

}
