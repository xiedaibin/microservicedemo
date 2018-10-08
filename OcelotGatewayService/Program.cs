using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.Middleware;

namespace OcelotGatewayService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            IWebHostBuilder builder = WebHost.CreateDefaultBuilder(args);
            builder.ConfigureServices(s =>
            {
                s.AddSingleton(builder);
            });
            builder
                 .ConfigureLogging((hostingContext, logging) =>
                 {
                     //add your logging
                 })
                 .UseKestrel()
                 .UseUrls("http://*:6800")
                 .ConfigureAppConfiguration(configurationbuilder =>
                 {
                     configurationbuilder.AddJsonFile("appsettings.json");
                     configurationbuilder.AddJsonFile("configuration.json");
                 })
                 .UseStartup<Startup>();
            return builder;
        }

    }
}
