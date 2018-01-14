using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Builder;
using Nowin;
using Owin;
using System.IO;
using System.Text;
using Model.DataBase;
using System.Web;
using Model;
using Model.Field;

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
        public void index(IOwinContext owinContext)
        {
            var response = owinContext.Response;
            var request = owinContext.Request;
            if (request.Method == "GET")
            {
                response.StatusCode = 200;
                response.ContentType = "text/html;charset=UTF-8";

                response.Write(File.ReadAllBytes("./index.html"));

            }
            else if (request.Method == "POST")
            {
                response.StatusCode = 200;
                response.ContentType = "text/plain;charset=UTF-8";
                using (var sr = new StreamReader(request.Body))
                {
                    try
                    {
                        var database = DataBaseManager.CreateSingleInstace();
                        database.Server = ".";
                        database.UserName = "sa";
                        database.PassWord = "123456";
                        database.SqlType = SqlType.MsSql;
                        database.DataBaseName = "master";
                        database.ModelManager.Register(new TestModel());
                        var r = sr.ReadToEnd();
                        var forms = r.Split('&');
                        var dic = new Dictionary<string, string>();
                        foreach (var name in forms)
                        {
                            var param = name.Split('=');
                            dic.Add(param[0], HttpUtility.UrlDecode(param[1]));
                        }

                        if (dic.ContainsKey("create"))
                        {
                            database.Create(dic["databasename"]);
                            response.Write("完成创建数据库");
                        }
                        else
                        {
                            database.Upgrade(dic["databasename"]);
                            response.Write("完成升级数据库");
                        }

                    }
                    catch (Exception ex)
                    {
                        response.Write($"系统异常：{ex.Message}");
                    }
                }
            }

        }

        public void Configuration(IAppBuilder app)
        {
            app.Run(async c =>
            {
                var path = c.Request.Path.Value;
                if (path == "/" || path == "/index.html")
                {
                    index(c);
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

    public class TestModel : RealModel
    {
        public IntegerField No { get; protected set; }
        public StringField Name { get; protected set; }
        public DecimalField Price { get; protected set; }
        public Many2One ParentId { get; protected set; }
        public StringField Title { get; protected set; }
        public TestModel() : base()
        {
            ModelName = "test.user";
            No = FieldFactory.CreateIntegerField(this, nameof(No));
            Name = FieldFactory.CreateStringField(this, nameof(Name), 100);
            Title = FieldFactory.CreateStringField(this, nameof(Title), 1000);
            Price = FieldFactory.CreateDecimalField(this, nameof(Price), 10, 2);
            ParentId = FieldFactory.CreateMany2OneField(this, nameof(ParentId), "test.parent");
        }
    }
}