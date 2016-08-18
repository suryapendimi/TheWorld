using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld
{
    public class Startup
    {
        private IHostingEnvironment _env;
        private IConfigurationRoot _config;
        public Startup(IHostingEnvironment env)
        {
            _env = env; //setup a Envirment varible on 

            var builder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile("config.json");
            _config = builder.Build();
                
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config =>
            {
                if(_env.IsProduction()) //if it is Prod then redirect to https SSL
                { 
                config.Filters.Add(new RequireHttpsAttribute());
                }
            });
            services.AddSingleton(_config);
            if (_env.IsEnvironment("Development")||_env.IsEnvironment("Testing"))
            {
                //Registering custom service to do 
                services.AddScoped<IMailService, DebugMailService>();
            }
            else
            {
                //implement actual service
            }
            services.AddDbContext<WorldContext>();
            services.AddScoped<IWorldRepository, WorldRepository>();
            services.AddTransient<GeoCoordsService>();
            services.AddTransient<WorldContextSeedData>();
            services.AddLogging();
            services.AddMvc()
                .AddJsonOptions(config =>
                {
                    config.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                }
                );
            services.AddIdentity<WorldUser, IdentityRole>(config =>
             {
                 config.User.RequireUniqueEmail = true;
                 //config.Password.RequiredLength = 8;
                 config.Cookies.ApplicationCookie.LoginPath = "/auth/login";
                 config.Cookies.ApplicationCookie.Events = new CookieAuthenticationEvents
                 {
                     OnRedirectToLogin = async ctx =>
                       {
                           if (ctx.Request.Path.StartsWithSegments("/api") &&
                           ctx.Response.StatusCode == 200)
                           {
                               ctx.Response.StatusCode = 401;
                           }
                           else
                           {
                               ctx.Response.Redirect(ctx.RedirectUri);
                           }
                           await Task.Yield();
                       }
                 };
             })
            .AddEntityFrameworkStores<WorldContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app,
            IHostingEnvironment env, WorldContextSeedData seeder,ILoggerFactory loggerFactory)
        {
            //This is to initialize
            Mapper.Initialize(config =>
            {
                //config.CreateMap<TripViewModel, Trip>(); //this to set map from TripViewModel to Trip(DB entity)

                config.CreateMap<TripViewModel, Trip>().ReverseMap(); //this to set map from Trip to TripViewModel (DB entity) 
                config.CreateMap<StopViewModel, Stop>().ReverseMap(); //this to set map from Trip to TripViewModel (DB entity) 
            }); 

            //app.UseDefaultFiles();
            if(env.IsEnvironment("Development"))
            { 
            //#if DEBUG
            app.UseDeveloperExceptionPage();
                //#endif
                loggerFactory.AddDebug(LogLevel.Information);
            }
            else
            {
                loggerFactory.AddDebug(LogLevel.Error);
            }

            app.UseStaticFiles();

            //turn on Identity
            app.UseIdentity();

            app.UseMvc(config =>
            {
                config.MapRoute(name: "Default",
                template: "{controller}/{action}/{id?}",
                defaults: new { controller = "App", action = "Index" }
                );
            });

            seeder.EnsureSeedData().Wait();
                
        }
    }
}
