using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Text;

namespace Rmq.Consumers
{
    public class RabbitMqOptions
    {
        public ConnectionFactory Factory;

        private IServiceScopeFactory scopeFactory;

        public RabbitMqOptions(string host, int port)
        {
            Factory = new ConnectionFactory() { HostName = host, Port = port };
        }

        public void SetScopeFactory(IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Creates a new RabbitMq Consumer and delegates the message to the appropriate handler
        /// </summary>
        /// <typeparam name="T">The Type of the Message received in this queue</typeparam>
        /// <typeparam name="THandler">The Type of the Handler to delegate this message to</typeparam>
        /// <param name="exchange"></param>
        /// <param name="queue"></param>
        /// <param name="topic"></param>
        /// <returns></returns>
        public RabbitMqOptions AddRabbitMQConsumer<T, THandler>(string exchange, string queue, string topic)
            where T : class
            where THandler : class, IRmqHandler<T>
        {
            var connection = Factory.CreateConnection();
            var channel = connection.CreateModel();

            Dictionary<string, object> bindingOneHeaders = new Dictionary<string, object>();
            bindingOneHeaders.Add("x-match", "type");
            bindingOneHeaders.Add("type", typeof(T).Name);

            channel.ExchangeDeclare(exchange, ExchangeType.Headers);
            channel.QueueDeclare(queue, true, false, false);
            channel.QueueBind(queue, exchange, topic, bindingOneHeaders);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    T obj = JsonConvert.DeserializeObject<T>(message);
                    THandler handler = scope.ServiceProvider.GetRequiredService<THandler>();
                    handler.Handle(obj, new System.Threading.CancellationToken());
                }
            };

            channel.BasicConsume(queue: queue,
                     autoAck: true,
                     consumer: consumer);

            return this;
        }

        public RabbitMqOptions AddRabbitMQConsumer<T, THandler>()
            where T : class
            where THandler : class, IRmqHandler<T>
        {
            var connection = Factory.CreateConnection();
            var channel = connection.CreateModel();

            Dictionary<string, object> bindingOneHeaders = new Dictionary<string, object>();
            bindingOneHeaders.Add("x-match", "all");
            bindingOneHeaders.Add("type", typeof(T).Name);

            channel.ExchangeDeclare("demo", ExchangeType.Headers, true);
            channel.QueueDeclare(typeof(T).Name, true, false, false);
            channel.QueueBind(typeof(T).Name, "demo", "", bindingOneHeaders);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    T obj = JsonConvert.DeserializeObject<T>(message);
                    THandler handler = scope.ServiceProvider.GetRequiredService<THandler>();
                    handler.Handle(obj, new System.Threading.CancellationToken());
                }
            };

            channel.BasicConsume(queue: typeof(T).Name,
                     autoAck: true,
                     consumer: consumer);

            return this;
        }
    }
}
