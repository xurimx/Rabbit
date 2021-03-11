using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rmq.Consumers;
using System.Reflection;
using DemoApp.Contracts;
using DemoReceiver.Worker.Handlers;

namespace DemoReceiver.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddRmqHandlers(Assembly.GetExecutingAssembly());

                    services.AddRabbitMq("broker", 5672, options => {
                        options.AddRabbitMQConsumer<TestEvent, TestEventHandler>();
                    });

                    services.AddHostedService<Worker>();
                });
    }
}
