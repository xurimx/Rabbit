using DemoApp.Contracts;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DemoPublisher
{
    public class RabbitPublisher : IRabbitPublisher
    {
        private readonly IConnection connection;

        public RabbitPublisher(IConnection connection)
        {
            this.connection = connection;
        }

        public Task Publish(IMessage message)
        {
            using (var model = connection.CreateModel())
            {
                model.ExchangeDeclare("demo", ExchangeType.Headers, true);
                model.QueueDeclare(message.GetType().Name, true, false, false, null);

                IBasicProperties props = model.CreateBasicProperties();
                props.Headers = new Dictionary<string, object>();
                props.Headers.Add("type", message.GetType().Name);
                props.ContentType = "application/json";

                var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));

                model.BasicPublish(exchange: "demo",
                     routingKey: "",
                     basicProperties: props,
                     body: body);
            }
            return Task.CompletedTask;
        }
    }
}
