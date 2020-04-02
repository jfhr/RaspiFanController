using Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace RaspiFanController
{
    public class Startup
    {
        public Startup(IWebHostEnvironment hostingEnvironment)
        {
            Configuration = new ConfigurationBuilder()
                            .SetBasePath(hostingEnvironment.ContentRootPath)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .Build();

            HostingEnvironment = hostingEnvironment;
        }

        public IConfigurationRoot Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<TemperatureController>();

            if (HostingEnvironment.IsDevelopment())
            {
                services.AddSingleton<ITemperatureProvider, RandomTemperatureProvider>();
            }
            else
            {
                services.AddSingleton<ITemperatureProvider, RealTemperatureProvider>();
            }
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            app.ApplicationServices.GetService<TemperatureController>().StartTemperatureMeasurement();
        }
    }
}