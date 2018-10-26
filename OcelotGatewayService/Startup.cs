using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;
using Ocelot.Provider.Polly;
using Swashbuckle.AspNetCore.Swagger;

namespace OcelotGatewayService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            #region 配置IdentityService4认证服务器
            //基础服务
            var authenticationBasicsServiceKey = "BasicsServiceKey";
            Action<IdentityServerAuthenticationOptions> BasicsServiceOptions = o =>
            {
                o.Authority = "http://localhost:5000";
                o.ApiName = "BasicsServiceApi";
                o.SupportedTokens = SupportedTokens.Both;
                o.ApiSecret = "secret";
            };
            //存货服务
            var authenticationInvoicingServiceKey = "InvoicingServiceKey";
            Action<IdentityServerAuthenticationOptions> InvoicingServiceOptions = o =>
            {
                o.Authority = "http://localhost:5000";
                o.ApiName = "InvoicingServiceApi";
                o.SupportedTokens = SupportedTokens.Both;
                o.ApiSecret = "secret";
            };

            //注入到认证中
            services.AddAuthentication()
                .AddIdentityServerAuthentication(authenticationBasicsServiceKey, BasicsServiceOptions)
                .AddIdentityServerAuthentication(authenticationInvoicingServiceKey, BasicsServiceOptions);

            #endregion


            //设置Ocelot相关配置  OcelotPolly 依赖Polly
            //设置 注册发现
            services
                .AddOcelot(Configuration)
                .AddConsul()
                .AddPolly();

            #region 设置swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc($"{Configuration["Swagger:DocName"]}", new Info
                {
                    Title = Configuration["Swagger:Title"],
                    Version = Configuration["Swagger:Version"]
                });
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //使用Mvc以及 swagger
            var apiList = Configuration["Swagger:ServiceDocNames"].Split(',').ToList();
            app.UseMvc()
               .UseSwagger()
               .UseSwaggerUI(options =>
               {
                   apiList.ForEach(apiItem =>
                   {
                       options.SwaggerEndpoint($"/doc/{apiItem}/swagger.json", apiItem);
                   });
               });
            //使用Ocelot
            await app.UseOcelot();
            
        }
    }
}
