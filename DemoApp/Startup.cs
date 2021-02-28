using DemoApp.Contracts;
using DemoApp.Services;
using DemoPublisher;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Rmq.Consumers;

namespace DemoApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        
        public void ConfigureServices(IServiceCollection services)
        {            
            services.AddControllers();

            services.AddRmqHandlers(GetType().Assembly);

            services.AddRabbitMq("broker", 5672, options => {
                options.AddRabbitMQConsumer<Message, MessageHandler>("mediator", "worker", "message");
                options.AddRabbitMQConsumer<TestEvent, TestEventHandler>("mediator", "worker", "testevent");
            });

            services.AddScoped<IRabbitPublisher, RabbitPublisher>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer("Data Source=database; Initial Catalog=postdb; User Id=sa; Password=P@ssw0rd;", options => {
                    options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
