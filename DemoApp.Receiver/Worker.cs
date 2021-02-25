using DemoApp.Contracts;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DemoApp.Receiver
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IMediator mediator;

        public Worker(ILogger<Worker> logger, IMediator mediator)
        {
            _logger = logger;
            this.mediator = mediator;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory() { HostName = "broker" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("mediator", ExchangeType.Fanout);
            channel.QueueDeclare("worker", true, false, false);
            channel.QueueBind("worker", "mediator", "");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                object type = null;
                ea.BasicProperties.Headers?.TryGetValue("MessageType", out type);

                if (type != null)
                {
                    IMessage cmd = MessageFactory.Create(message, Enum.Parse<MessageType>(type.ToString()));

                    var concreteType = cmd.GetType();
                    Type generic = typeof(GenericRequest<>);
                    Type[] typeArgs = { concreteType };
                    Type constructed = generic.MakeGenericType(typeArgs);
                    var instance = Activator.CreateInstance(constructed, cmd);

                    mediator.Send(instance);
                }
            };

            channel.BasicConsume(queue: "worker",
                                 autoAck: true,
                                 consumer: consumer);

            Console.WriteLine("Subscribed!!");
                                  
        }
    }
}
