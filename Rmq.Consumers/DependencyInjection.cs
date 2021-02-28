using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Rmq.Consumers
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds Message Handlers to dependency injection container, this call is necessary to add RabbitMQ Consumers
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IServiceCollection AddRmqHandlers(this IServiceCollection services, params Assembly[] assemblies)
        {
            var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.GetInterface(typeof(IRmqHandler<>).Name.ToString()) != null));

            foreach (var type in typesFromAssemblies)
                services.AddTransient(type);

            return services;
        }

        /// <summary>
        /// Adds IConnection to depencency injection as Singleton
        /// </summary>
        /// <param name="services"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, string host, int port)
        {
            var factory = new ConnectionFactory() { HostName = host, Port = port };

            services.AddSingleton(_ => factory.CreateConnection());

            return services;
        }

        /// <summary>
        /// Adds IConnection to depencency injection as Singleton and takes in Options to Add Consumers and their Message Handlers,
        /// RabbitMQ Consumers are created after the generic host has started.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMq(this IServiceCollection services, string host, int port, Action<RabbitMqOptions> action)
        {
            var rmqBuilder = new RabbitMqOptions(host, port);
            services.AddSingleton(_ => rmqBuilder.Factory.CreateConnection());

            services.AddHostedService(x =>
            {
                var sf = x.GetRequiredService<IServiceScopeFactory>();
                var logger = x.GetRequiredService<ILogger<RmqHostedService>>();
                return new RmqHostedService(logger, sf, action, rmqBuilder);
            });

            return services;
        }
        internal class RmqHostedService : IHostedService
        {
            private readonly ILogger<RmqHostedService> logger;
            private readonly IServiceScopeFactory scopeFactory;
            private readonly Action<RabbitMqOptions> action;
            private readonly RabbitMqOptions rmqBuilder;

            public RmqHostedService(ILogger<RmqHostedService> logger, IServiceScopeFactory scopeFactory, Action<RabbitMqOptions> action, RabbitMqOptions rmqBuilder)
            {
                this.logger = logger;
                this.scopeFactory = scopeFactory;
                this.action = action;
                this.rmqBuilder = rmqBuilder;
            }
            public Task StartAsync(CancellationToken cancellationToken)
            {
                logger.LogInformation("Creating RabbitMq Consumers!");
                rmqBuilder.SetScopeFactory(scopeFactory);
                action(rmqBuilder);
                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                logger.LogInformation("Stopping internal RabbitMq Hosted Service!");
                return Task.CompletedTask;
            }
        }
    }
}
