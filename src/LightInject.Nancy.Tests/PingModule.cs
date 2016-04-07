namespace LightInject.Nancy.Tests
{
    using global::Nancy;
    public class PingModule : NancyModule
    {
        public PingModule()
        {            
            Get["/ping"] = _ => "pong";
        }
    }
}