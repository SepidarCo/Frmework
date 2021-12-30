using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using Sepidar.BaseApi.Filters;
using Sepidar.Mvc;

namespace Sepidar.BaseApi
{
    public abstract class GeneralStartup
    {
        private static string allowedSpecificOrigins = "AllowedSpecificOrigins";
        public IConfiguration Configuration { get; }

        public GeneralStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            RegisterServices(services);
        }

        private void RegisterServices(IServiceCollection services)
        {
            RegisterDbContext(services);
            RegisterRepositories(services);
            RegisterBusinesses(services);

            services.AddHttpContextAccessor();

            RegisterCors(services);
            RegisterMvc(services);
        }

        protected abstract void RegisterDbContext(IServiceCollection services);

        protected abstract void RegisterRepositories(IServiceCollection services);

        protected abstract void RegisterBusinesses(IServiceCollection services);

        private void RegisterCors(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        if (Config.HasSetting("Security:CorsAllowedOrigins"))
                        {
                            var origins = Sepidar.Framework.Config.GetSetting("Security:CorsAllowedOrigins").Split(",");

                            builder.WithOrigins(origins)
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                        }
                    });
            });
        }

        private void RegisterMvc(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ResponseFilter));
                options.EnableEndpointRouting = false;
                options.ModelBinderProviders.Insert(0, new ListOptionsModelBinderProvider());
            });


            services.AddMvc()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ContractResolver =
                        new DefaultContractResolver());
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            ConfigureHttpContext(app);

            app.UseCors();

            ConfigureRoutes(app);


        }

        private void ConfigureHttpContext(IApplicationBuilder app)
        {
            AppHttpContext.Configure(app.ApplicationServices.GetRequiredService<IHttpContextAccessor>());
        }

        private void ConfigureRoutes(IApplicationBuilder app)
        {
            app.UseMvc(routes =>
            {
                routes.MapAreaRoute("EndUserArea", "EndUser", "EndUser/{controller=Home}/{action=Index}/{id?}");
                routes.MapAreaRoute("AdminArea", "Admin", "Admin/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }

    }
}
