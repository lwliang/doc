using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin.Builder;
using Nowin;
using Owin;

namespace NowinSample
{
    static class Program
    {
        static void Main(string[] args)
        {
            var owinbuilder = new AppBuilder();
            OwinServerFactory.Initialize(owinbuilder.Properties);
            new SampleOwinApp.Startup().Configuration(owinbuilder);
            var builder = ServerBuilder.New()
                .SetPort(8888)
                .SetOwinApp(owinbuilder.Build())
                .SetOwinCapabilities((IDictionary<string, object>)owinbuilder.Properties[OwinKeys.ServerCapabilitiesKey])
                .SetExecutionContextFlow(ExecutionContextFlow.SuppressAlways);
            //builder
            //    .SetCertificate(new X509Certificate2("../../../sslcert/test.pfx", "nowin"))
            //    .RequireClientCertificate();
            using (var server = builder.Build())
            {
                // Workaround for bug in Windows Server 2012 when ReadLine is called directly after AcceptAsync
                // By starting it in another thread and probably even later than calling readline it works
                Task.Run(() => server.Start());
                //using (new Timer(o =>
                //    {
                //        var s = (INowinServer)o;
                //        Console.WriteLine("Connections {0}/{1}", s.ConnectionCount, s.CurrentMaxConnectionCount);
                //    }, server, 2000, 2000))
                {
                    Console.WriteLine("Listening on ports 8888. Enter to exit.");
                    Console.ReadLine();
                }
            }
        }
    }

}

namespace SampleOwinApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Run(async c =>
            {
                var path = c.Request.Path.Value;
                if (path == "/")
                {
                    c.Response.StatusCode = 200;
                    c.Response.ContentType = "text/plain";
                    c.Response.Write("Hello World!");
                    return;
                }
                if (path == "/sse")
                {
                    c.Response.StatusCode = 200;
                    c.Response.ContentType = "text/event-stream";
                    c.Response.Headers.Add("Cache-Control", new[] { "no-cache" });
                    for (int i = 0; i < 10; i++)
                    {
                        await c.Response.WriteAsync("data: " + i.ToString() + "\n\n");
                        await c.Response.Body.FlushAsync();
                        await Task.Delay(500);
                    }
                    await c.Response.WriteAsync("data: Finish!\n\n");
                    return;
                }
                c.Response.StatusCode = 404;
                return;
            });
        }
    }

    public static class Sample
    {
        static readonly Func<IDictionary<string, object>, Task> OwinApp;

        static Sample()
        {
            var builder = new AppBuilder();
            new Startup().Configuration(builder);
            OwinApp = builder.Build();
        }

        public static Task App(IDictionary<string, object> arg)
        {
            return OwinApp(arg);
        }
    }
}