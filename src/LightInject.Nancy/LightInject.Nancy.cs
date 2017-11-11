/*****************************************************************************   
    The MIT License (MIT)

    Copyright (c) 2014 bernhard.richter@gmail.com

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
******************************************************************************
    LightInject.Nancy version 2.0.0
    http://seesharper.github.io/LightInject/
    http://twitter.com/bernhardrichter    
******************************************************************************/
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1126:PrefixCallsCorrectly", Justification = "Reviewed")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "No inheritance")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1403:FileMayOnlyContainASingleNamespace", Justification = "Extension methods must be visible")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1633:FileMustHaveHeader", Justification = "Custom header.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "All public members are documented.")]

#if NETSTANDARD16
namespace System.Diagnostics.CodeAnalysis
{
    public class ExcludeFromCodeCoverageAttribute : Attribute
    { }
}
#endif

namespace LightInject.Nancy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using global::Nancy;
    using global::Nancy.Bootstrapper;
    using global::Nancy.Configuration;
    using global::Nancy.Diagnostics;

    /// <summary>
    /// A Nancy bootstrapper for LightInject.
    /// </summary>
    public class LightInjectNancyBootstrapper : NancyBootstrapperBase<IServiceContainer>
    {
        /// <summary>
        /// Get the <see cref="INancyEnvironment" /> instance.
        /// </summary>
        /// <returns>An configured <see cref="INancyEnvironment" /> instance.</returns>
        /// <remarks>The boostrapper must be initialised (<see cref="INancyBootstrapper.Initialise" />) prior to calling this.</remarks>
        public override INancyEnvironment GetEnvironment()
        {
            return ApplicationContainer.GetInstance<INancyEnvironment>();
        }

        /// <summary>
        /// Gets the <see cref="INancyEnvironmentConfigurator"/> used by th.
        /// </summary>
        /// <returns>An <see cref="INancyEnvironmentConfigurator"/> instance.</returns>
        protected override INancyEnvironmentConfigurator GetEnvironmentConfigurator()
        {
            return ApplicationContainer.GetInstance<INancyEnvironmentConfigurator>();
        }

        /// <summary>
        /// Registers an <see cref="INancyEnvironment"/> instance in the container.
        /// </summary>
        /// <param name="container">The container to register into.</param>
        /// <param name="environment">The <see cref="INancyEnvironment"/> instance to register.</param>
        protected override void RegisterNancyEnvironment(IServiceContainer container, INancyEnvironment environment)
        {
            container.RegisterInstance<INancyEnvironment>(environment);
        }

        /// <summary>
        /// Gets an <see cref="INancyModule"/> instance.
        /// </summary>
        /// <param name="moduleType">The type of <see cref="INancyModule"/> to get.</param>
        /// <param name="context">The current <see cref="NancyContext"/>.</param>
        /// <returns>An <see cref="INancyModule"/> instance.</returns>
        public override INancyModule GetModule(Type moduleType, NancyContext context)
        {
            EnsureScopeIsStarted(context);
            return ApplicationContainer.GetInstance<INancyModule>(moduleType.FullName);
        }

        /// <summary>
        /// Gets all <see cref="INancyModule"/> instances.
        /// </summary>
        /// <param name="context">The current <see cref="NancyContext"/>.</param>
        /// <returns>All <see cref="INancyModule"/> instances.</returns>
        public override IEnumerable<INancyModule> GetAllModules(NancyContext context)
        {
            EnsureScopeIsStarted(context);
            return ApplicationContainer.GetAllInstances<INancyModule>();
        }

        /// <summary>
        /// Gets the diagnostics for initialization.
        /// </summary>
        /// <returns>An <see cref="IDiagnostics"/> instance.</returns>
        protected override IDiagnostics GetDiagnostics()
        {
            return ApplicationContainer.GetInstance<IDiagnostics>();
        }

        /// <summary>
        /// Gets the <see cref="INancyEngine"/> instance.
        /// </summary>
        /// <returns><see cref="INancyEngine"/></returns>
        protected override INancyEngine GetEngineInternal()
        {
            return ApplicationContainer.GetInstance<INancyEngine>();
        }

        /// <summary>
        /// Initializes the <see cref="IServiceContainer"/> instance.
        /// </summary>
        /// <returns><see cref="IServiceContainer"/>.</returns>
        protected override IServiceContainer GetApplicationContainer()
        {
            return new ServiceContainer();
        }

        /// <summary>
        /// Registers the <see cref="INancyModuleCatalog"/> into the underlying <see cref="IServiceContainer"/> instance.
        /// </summary>
        /// <param name="applicationContainer">The <see cref="IServiceContainer"/> to register into.</param>
        protected override void RegisterBootstrapperTypes(IServiceContainer applicationContainer)
        {
            applicationContainer.ScopeManagerProvider = new PerLogicalCallContextScopeManagerProvider();
            applicationContainer.Register<Func<NancyContext>>(factory => () => NancyContextRequestStartup.Current, new PerContainerLifetime());
            applicationContainer.RegisterInstance<INancyModuleCatalog>(this);
            foreach (var requestStartupType in RequestStartupTasks)
            {
                applicationContainer.Register(typeof(IRequestStartup), requestStartupType, requestStartupType.FullName, new PerScopeLifetime());
            }
        }

        /// <summary>
        /// Registers the <paramref name="typeRegistrations"/> into the underlying <see cref="IServiceContainer"/>.
        /// </summary>
        /// <param name="container">The <see cref="IServiceContainer"/> to register into.</param>
        /// <param name="typeRegistrations">Each <see cref="TypeRegistration"/> represents a service to be registered.</param>
        protected override void RegisterTypes(IServiceContainer container, IEnumerable<TypeRegistration> typeRegistrations)
        {
            foreach (var typeRegistration in typeRegistrations)
            {
                switch (typeRegistration.Lifetime)
                {
                    case Lifetime.Transient:
                        RegisterTransient(container, typeRegistration.RegistrationType, typeRegistration.ImplementationType);
                        break;
                    case Lifetime.Singleton:
                        RegisterSingleton(container, typeRegistration.RegistrationType, typeRegistration.ImplementationType);
                        break;
                    case Lifetime.PerRequest:
                        RegisterPerRequest(container, typeRegistration.RegistrationType, typeRegistration.ImplementationType);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets all registered application startup tasks
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IApplicationStartup"/> instances.</returns>
        protected override IEnumerable<IApplicationStartup> GetApplicationStartupTasks()
        {
            return ApplicationContainer.GetAllInstances<IApplicationStartup>();
        }

        /// <summary>
        /// Gets all <see cref="IRequestStartup"/> instances.
        /// </summary>
        /// <param name="container">The target <see cref="IServiceContainer"/>.</param>
        /// <param name="requestStartupTypes">Not used in this method.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IRequestStartup"/> instances.</returns>        
        [ExcludeFromCodeCoverage]
        protected override IEnumerable<IRequestStartup> RegisterAndGetRequestStartupTasks(IServiceContainer container, Type[] requestStartupTypes)
        {
            return container.GetAllInstances<IRequestStartup>();
        }

        /// <summary>
        /// Gets all <see cref="IRegistrations"/> instances.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IRegistrations"/> instances.</returns>
        protected override IEnumerable<IRegistrations> GetRegistrationTasks()
        {
            return ApplicationContainer.GetAllInstances<IRegistrations>();
        }

        /// <summary>
        /// Registers multiple implementations of a given interface.
        /// </summary>
        /// <param name="container">The <see cref="IServiceContainer"/> to register into.</param>
        /// <param name="collectionTypeRegistrations">A list of <see cref="CollectionTypeRegistration"/> instances where each instance represents an abstraction and its implementations.</param>
        protected override void RegisterCollectionTypes(IServiceContainer container, IEnumerable<CollectionTypeRegistration> collectionTypeRegistrations)
        {
            foreach (var collectionTypeRegistration in collectionTypeRegistrations)
            {
                foreach (Type implementingType in collectionTypeRegistration.ImplementationTypes)
                {
                    switch (collectionTypeRegistration.Lifetime)
                    {
                        case Lifetime.Transient:
                            RegisterTransient(container, collectionTypeRegistration.RegistrationType, implementingType, implementingType.FullName);
                            break;
                        case Lifetime.Singleton:
                            RegisterSingleton(container, collectionTypeRegistration.RegistrationType, implementingType, implementingType.FullName);
                            break;
                        case Lifetime.PerRequest:
                            RegisterPerRequest(container, collectionTypeRegistration.RegistrationType, implementingType, implementingType.FullName);
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Register the given <paramref name="moduleRegistrationTypes"/> into the <paramref name="container"/>.
        /// </summary>
        /// <param name="container">The <see cref="IServiceContainer"/> to register into.</param>
        /// <param name="moduleRegistrationTypes">The list of <see cref="ModuleRegistration"/> that 
        /// represents an <see cref="INancyModule"/> registration.</param>
        protected override void RegisterModules(IServiceContainer container, IEnumerable<ModuleRegistration> moduleRegistrationTypes)
        {
            foreach (var moduleRegistrationType in moduleRegistrationTypes)
            {
                container.Register(typeof(INancyModule), moduleRegistrationType.ModuleType, moduleRegistrationType.ModuleType.FullName, new PerScopeLifetime());
            }
        }

        /// <summary>
        /// Register the given instances into the container
        /// </summary>
        /// <param name="container">The <see cref="IServiceContainer"/> to register into.</param>
        /// <param name="instanceRegistrations">Instance registration types</param>
        protected override void RegisterInstances(IServiceContainer container, IEnumerable<InstanceRegistration> instanceRegistrations)
        {
            foreach (var instanceRegistration in instanceRegistrations)
            {
                container.RegisterInstance(instanceRegistration.RegistrationType, instanceRegistration.Implementation);
            }
        }

        /// <summary>
        /// Gets all <see cref="IRequestStartup"/> instances from the <see cref="IServiceContainer"/>
        /// and calls the <see cref="IRequestStartup.Initialize"/> method.
        /// </summary>
        /// <param name="context">The current <see cref="NancyContext"/>.</param>
        /// <returns><see cref="IPipelines"/>.</returns>
        protected override IPipelines InitializeRequestPipelines(NancyContext context)
        {
            var pipelines = new Pipelines(ApplicationPipelines);
            EnsureScopeIsStarted(context);

            var requestStartupTasks = ApplicationContainer.GetAllInstances<IRequestStartup>();
            foreach (var requestStartupTask in requestStartupTasks)
            {
                requestStartupTask.Initialize(pipelines, context);
            }

            RequestStartup(ApplicationContainer, pipelines, context);

            return pipelines;
        }

        private void EnsureScopeIsStarted(NancyContext context)
        {
            object contextObject;
            context.Items.TryGetValue("LightInjectScope", out contextObject);
            var scope = contextObject as Scope;

            if (scope == null)
            {
                scope = ApplicationContainer.BeginScope();
                context.Items["LightInjectScope"] = scope;
            }
        }

        private void RegisterTransient(IServiceContainer container, Type serviceType, Type implementingType, string serviceName = "")
        {
            if (typeof(IDisposable).IsAssignableFrom(implementingType))
            {
                container.Register(serviceType, implementingType, serviceName, new PerRequestLifeTime());
            }
            else
            {
                container.Register(serviceType, implementingType, serviceName);
            }
        }

        private void RegisterPerRequest(IServiceContainer container, Type serviceType, Type implementingType, string serviceName = "")
        {
            container.Register(serviceType, implementingType, serviceName, new PerScopeLifetime());
        }

        private void RegisterSingleton(IServiceContainer container, Type serviceType, Type implementingType, string serviceName = "")
        {
            container.Register(serviceType, implementingType, serviceName, new PerContainerLifetime());
        }
    }

    /// <summary>
    /// An <see cref="IRequestStartup"/> class that captures the current <see cref="NancyContext"/>
    /// so that it can be injected into any class.
    /// </summary>
    public class NancyContextRequestStartup : IRequestStartup
    {
        private static readonly LogicalThreadStorage<NancyContextStorage> ContextStorage =
            new LogicalThreadStorage<NancyContextStorage>();

        /// <summary>
        /// Perform any initialisation tasks
        /// </summary>
        /// <param name="pipelines">Application pipelines</param><param name="context">The current context</param>
        public void Initialize(IPipelines pipelines, NancyContext context)
        {
            pipelines.BeforeRequest.AddItemToStartOfPipeline(nancyContext =>
            {
                ContextStorage.Value = new NancyContextStorage { Context = nancyContext };
                return context.Response;
            });

            pipelines.AfterRequest.AddItemToEndOfPipeline(nancyContext =>
                ContextStorage.Value = null);
        }

        /// <summary>
        /// Gets the current <see cref="NancyContext"/>.
        /// </summary>
        public static NancyContext Current => ContextStorage.Value.Context;

        private class NancyContextStorage
        {
            public NancyContext Context { get; set; }
        }
    }
}