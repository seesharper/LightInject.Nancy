namespace LightInject.Nancy.Tests
{
    using global::Nancy;
    using global::Nancy.Bootstrapper;
    public class SampleStartupTask : IRequestStartup
    {
        public void Initialize(IPipelines pipelines, NancyContext context)
        {
            
        }
    }
}