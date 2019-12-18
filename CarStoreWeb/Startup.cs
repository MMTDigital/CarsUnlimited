using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarStoreWeb.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CarStoreWeb
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
            services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromHours(6);
                    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.None;
                    options.Cookie.Name = "CarsUnlimited.Session";

                    options.Cookie.HttpOnly = true;
                }
            );
            services.AddControllersWithViews();
            services.AddDistributedMemoryCache();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            var cartApi = Configuration.GetSection("CartAPI").Value;
            var inventoryApi = Configuration.GetSection("InventoryApi").Value;
            var purchaseApi = Configuration.GetSection("PurchaseApi").Value;

            var cartApiKey = Configuration.GetSection("CartApiKey").Value;
            var inventoryApiKey = Configuration.GetSection("InventoryApiKey").Value;
            var purchaseApiKey = Configuration.GetSection("PurchaseApiKey").Value;

            new CartProxy(cartApi, cartApiKey);
            new InventoryProxy(inventoryApi, inventoryApiKey);
            new PurchaseProxy(purchaseApi, purchaseApiKey);
        }
    }
}
