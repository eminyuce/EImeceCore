using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EImeceCore.Web.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity.UI.Services;
using EImeceCore.Domain.Services;
using DbInfrastructure.Services;
using DbInfrastructure.Services.IServices;
using EImeceCore.Domain;
using DbInfrastructure.EFContext;
using DbInfrastructure.Repositories.IRepositories;
using System.Reflection;
using DbInfrastructure.Repositories;
using System.Text;

namespace EImeceCore.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private IServiceCollection _services;
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddLogging();
            // Add memory cache services
            services.AddMemoryCache();

      

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
           

            // services.AddDbContext<ProjectDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
            services.AddTransient<IProjectDbContext>(s => new ProjectDbContext(Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>().AddEntityFrameworkStores<ApplicationDbContext>();
            AddTransientByReflection(services, typeof(IBaseService<>), "Service");
            AddTransientByReflection(services, typeof(IBaseRepository<>), "Repository");

    

            services.AddSingleton<MyAppSetttings>();
           
            services.AddTransient<IEmailSender, EmailSender>(i =>
              new EmailSender(
                  Configuration["EmailSender:Host"],
                  Configuration.GetValue<int>("EmailSender:Port"),
                  Configuration.GetValue<bool>("EmailSender:EnableSSL"),
                  Configuration["EmailSender:UserName"],
                  Configuration["EmailSender:Password"]
              )
          );

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            _services = services;
        }

       
        private static void AddTransientByReflection(IServiceCollection services, Type typeOfInterface, string typeofText)
        {
            var baseServiceTypes = Assembly.GetAssembly(typeOfInterface)
               .GetTypes().Where(t => t.Name.EndsWith(typeofText)).ToList();

            foreach (var type in baseServiceTypes)
            {
                string baseClass = "I" + type.Name;
                if (!baseClass.StartsWith("IBase"))
                {
                    var interfaceType = type.GetInterface(baseClass);
                    if (interfaceType != null)
                    {
                        services.AddScoped(interfaceType, type);
                    }
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                ListAllRegisteredServices(app);
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }


        private void ListAllRegisteredServices(IApplicationBuilder app)
        {
            app.Map("/allservices", builder => builder.Run(async context =>
            {
                var sb = new StringBuilder();
                sb.Append("<h1>All Services</h1>");
                sb.Append("<table><thead>");
                sb.Append("<tr><th>Type</th><th>Lifetime</th><th>Instance</th></tr>");
                sb.Append("</thead><tbody>");
                foreach (var svc in _services)
                {
                    sb.Append("<tr>");
                    sb.Append($"<td>{svc.ServiceType.FullName}</td>");
                    sb.Append($"<td>{svc.Lifetime}</td>");
                    sb.Append($"<td>{svc.ImplementationType?.FullName}</td>");
                    sb.Append("</tr>");
                }
                sb.Append("</tbody></table>");
                await context.Response.WriteAsync(sb.ToString());
            }));
        }
    }
}
