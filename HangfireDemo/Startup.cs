using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPipes;
using Hangfire;
using Hangfire.MemoryStorage;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HangfireDemo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHangfire(config => config.UseMemoryStorage());
            services.AddHangfireServer();

            services.AddMassTransit(x =>
            {
                x.AddConsumer<TicketConsumer>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.UseHealthCheck(provider);
                    cfg.Host(new Uri("rabbitmq://localhost"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint("orderTicketQueue", ep =>
                    {
                        ep.PrefetchCount = 10;
                        ep.UseMessageRetry(r => r.Interval(2, 100));
                        ep.ConfigureConsumer<TicketConsumer>(provider);
                    });

                }));
            });

            services.AddMassTransitHostedService();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });

            app.UseHangfireDashboard();
            //backgroundJobClient.Enqueue(() => Console.WriteLine("Eaaaaae home :)"));
            //recurringJobManager.AddOrUpdate("Run every minute...", () => Console.WriteLine("Test recurring job"), Cron.Minutely);
            recurringJobManager.AddOrUpdate("tested...", () => Console.WriteLine("Olha só..."), "*/5 * * * * *");
        }
    }
}
