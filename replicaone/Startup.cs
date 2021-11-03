using System;
using System.Linq;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace replicaone
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddDapr();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            // Register a hook to set up application on startup
            lifetime.ApplicationStarted.Register(() => OnApplicationStarted(app));
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCloudEvents();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapSubscribeHandler();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }

        public async void OnApplicationStarted(IApplicationBuilder app)
        {
            var proxyId = ActorId.CreateRandom();
            Console.WriteLine($"Creating actor proxy: {proxyId}");
            var proxy = ActorProxy.Create<owner.ITestActor>(proxyId, "TestActor");

            var retries = 0;
            while (true)
            {
                try
                {

                    Console.WriteLine($"Requesting data from proxy: {proxyId}");
                    await proxy.PublishTestDataAsync();
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Boot failed: {ex}");
                    retries++;
                    if (retries >= 10)
                        throw;

                    // Random exponential back-off
                    var secs = (int)Math.Max(1, Math.Min(100, Math.Pow(1 + new Random().NextDouble(), retries)));
                    Console.WriteLine($"Retry #{retries} in {secs}s");
                    await Task.Delay(TimeSpan.FromSeconds(secs));
                }
            }
        }
    }
}
